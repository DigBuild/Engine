using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.BuiltIn.GeneratedUniforms;
using DigBuild.Engine.Events;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render.Worlds
{
    public sealed class ChunkWorldRenderer : IWorldRenderer
    {
        private readonly IReadOnlyWorld _world;
        private readonly EventBus _eventBus;
        private readonly Func<IReadOnlyChunk, ImmutableList<IChunkRenderer>> _chunkRendererFactory;

        private readonly Dictionary<ChunkPos, ChunkRenderData> _chunkRenderData = new();
        
        private ChunkPos _currentCameraPos = new(int.MaxValue, int.MaxValue);
        private List<ChunkPos> _sortedChunks = new();
        
        private readonly HashSet<ChunkPos> _trackedChunks = new();
        private readonly HashSet<ChunkPos> _loadedChunks = new();
        private readonly HashSet<ChunkPos> _unloadedChunks = new();

        private readonly Dictionary<ChunkPos, UniformBufferSet.Snapshot> _renderedChunkUniforms = new();

        public ChunkWorldRenderer(
            IReadOnlyWorld world,
            EventBus eventBus,
            Func<IReadOnlyChunk, ImmutableList<IChunkRenderer>> chunkRendererFactory
        )
        {
            _world = world;
            _eventBus = eventBus;
            _chunkRendererFactory = chunkRendererFactory;
            
            _eventBus.Subscribe<BuiltInChunkEvent.Loaded>(OnChunkLoaded);
            _eventBus.Subscribe<BuiltInChunkEvent.Unloading>(OnChunkUnloaded);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<BuiltInChunkEvent.Loaded>(OnChunkLoaded);
            _eventBus.Unsubscribe<BuiltInChunkEvent.Unloading>(OnChunkUnloaded);

            foreach (var data in _chunkRenderData.Values)
                data.Dispose();
            _chunkRenderData.Clear();

            _renderedChunkUniforms.Clear();
        }

        private void OnChunkLoaded(BuiltInChunkEvent.Loaded evt)
        {
            lock (_loadedChunks)
            {
                _loadedChunks.Add(evt.Chunk.Position);
            }
        }

        private void OnChunkUnloaded(BuiltInChunkEvent.Unloading evt)
        {
            lock (_unloadedChunks)
            {
                _unloadedChunks.Add(evt.Chunk.Position);
            }
        }

        private ChunkRenderData? GetOrCreateData(ChunkPos pos)
        {
            if (_chunkRenderData.TryGetValue(pos, out var data))
                return data;

            var chunk = _world.GetChunk(pos, false);
            if (chunk == null)
                return null;
            var renderers = _chunkRendererFactory(chunk);
            return _chunkRenderData[pos] = new ChunkRenderData(chunk, renderers);
        }

        public void Update(RenderContext context, WorldView worldView, float partialTick)
        {
            // Check if we need to sort all the chunks
            var cameraPos = worldView.Camera.Position;
            var cameraChunkPos = new BlockPos(cameraPos).ChunkPos;
            var needsSort = cameraChunkPos != _currentCameraPos;

            // Add all loaded chunks
            lock (_loadedChunks)
            {
                var chunksBefore = _trackedChunks.Count;
                _trackedChunks.UnionWith(_loadedChunks);
                needsSort |= _trackedChunks.Count != chunksBefore;
                _loadedChunks.Clear();
            }

            // Remove all unloaded chunks and their associated data
            lock (_unloadedChunks)
            {
                foreach (var pos in _unloadedChunks)
                {
                    if (!needsSort)
                        _sortedChunks.Remove(pos);
                    if (_chunkRenderData.Remove(pos, out var data))
                        data.Dispose();
                }
                _trackedChunks.ExceptWith(_unloadedChunks);
                _unloadedChunks.Clear();
            }

            // Sort from closest to the camera to farthest if needed
            if (needsSort)
            {
                _currentCameraPos = cameraChunkPos;
                _sortedChunks = _trackedChunks.OrderBy(pos => pos.DistanceSq(cameraChunkPos)).ToList();
            }

            // Update all the chunks
            var sortedData = _sortedChunks.Select(GetOrCreateData).ToList();
            var heavyUpdate = sortedData.Where(d => d?.HasHeavyUpdate ?? false).Take(4).ToImmutableHashSet();
            Parallel.ForEach(sortedData, d =>
            {
                d?.Update(!heavyUpdate.Contains(d), worldView, partialTick);
            });
            foreach (var d in sortedData)
                d?.Upload(context, worldView, partialTick);
        }

        public void BeforeDraw(RenderContext context, CommandBufferRecorder cmd, UniformBufferSet uniforms,
            WorldView worldView, float partialTick)
        {
            _renderedChunkUniforms.Clear();

            var cameraTransform = worldView.Camera.Transform;

            foreach (var data in _sortedChunks.Select(GetOrCreateData))
            {
                if (data == null)
                    continue;
                if (!worldView.ViewFrustum.Test(data.Bounds))
                    continue;

                uniforms.Push(BuiltInRenderUniforms.ModelViewProjectionTransform, new SimpleTransform
                {
                    ModelView = Matrix4x4.CreateTranslation(data.Position.GetOrigin()) * cameraTransform,
                    Projection = worldView.Projection
                });
                _eventBus.Post(new PushChunkUniformsEvent(data.Chunk, uniforms));
                _renderedChunkUniforms[data.Position] = uniforms.CaptureSnapshot();

                data.BeforeDraw(context, cmd, uniforms, worldView, partialTick);
            }
        }

        public void Draw(RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer, RenderLayerBindingSet bindings,
            IReadOnlyUniformBufferSet uniforms, WorldView worldView, float partialTick)
        {
            foreach (var (pos, uniformSnapshot) in _renderedChunkUniforms)
                _chunkRenderData[pos].Draw(context, cmd, layer, bindings, uniformSnapshot, worldView, partialTick);
        }

        public void AfterDraw(RenderContext context, CommandBufferRecorder cmd, WorldView worldView, float partialTick)
        {
            foreach (var pos in _renderedChunkUniforms.Keys)
                _chunkRenderData[pos].AfterDraw(context, cmd, worldView, partialTick);
        }

        private sealed class ChunkRenderData : IDisposable
        {
            private readonly ImmutableList<IChunkRenderer> _renderers;

            public IReadOnlyChunk Chunk { get; }
            public ChunkPos Position { get; }
            public AABB Bounds { get; }

            public ChunkRenderData(IReadOnlyChunk chunk, ImmutableList<IChunkRenderer> renderers)
            {
                Chunk = chunk;
                Position = chunk.Position;
                _renderers = renderers;
                var min = Position.GetOrigin();
                var max = min + new Vector3(WorldDimensions.ChunkWidth, WorldDimensions.ChunkHeight, WorldDimensions.ChunkWidth);
                Bounds = new AABB(min, max);
            }

            public void Dispose()
            {
                foreach (var renderer in _renderers)
                    renderer.Dispose();
            }

            public bool HasHeavyUpdate => _renderers.Any(r => r.HasHeavyUpdate);

            public void Update(bool express, WorldView worldView, float partialTick)
            {
                foreach (var renderer in _renderers)
                    renderer.Update(express, worldView, partialTick);
            }

            public void Upload(RenderContext context, WorldView worldView, float partialTick)
            {
                foreach (var renderer in _renderers)
                    renderer.Upload(context, worldView, partialTick);
            }

            public void BeforeDraw(RenderContext context, CommandBufferRecorder cmd, UniformBufferSet uniforms, WorldView view, float partialTick)
            {
                foreach (var renderer in _renderers)
                    renderer.BeforeDraw(context, cmd, uniforms, view, partialTick);
            }

            public void Draw(
                RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer, RenderLayerBindingSet bindings,
                IReadOnlyUniformBufferSet uniforms, WorldView view, float partialTick
            )
            {
                foreach (var renderer in _renderers)
                    renderer.Draw(context, cmd, layer, bindings, uniforms, view, partialTick);
            }

            public void AfterDraw(RenderContext context, CommandBufferRecorder cmd, WorldView view, float partialTick)
            {
                foreach (var renderer in _renderers)
                    renderer.AfterDraw(context, cmd, view, partialTick);
            }
        }
    }
}