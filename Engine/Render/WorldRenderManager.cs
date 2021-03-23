using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class WorldRenderManager
    {
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly IReadOnlyDictionary<Entity, IEntityModel> _entityModels;
        private readonly IEnumerable<IRenderLayer> _renderLayers;
        private readonly NativeBufferPool _pool;
        private readonly UniformBufferSet _ubs;
        private readonly GeometryBufferSet _entityGbs;
        
        private readonly HashSet<IChunk> _updatedChunks = new();
        private readonly HashSet<IChunk> _removedChunks = new();
        private readonly Dictionary<IChunk, ChunkRenderData> _chunkRenderData = new();
        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public WorldRenderManager(
            IReadOnlyDictionary<Block, IBlockModel> blockModels,
            IReadOnlyDictionary<Entity, IEntityModel> entityModels,
            IEnumerable<IRenderLayer> renderLayers, NativeBufferPool pool
        )
        {
            _blockModels = blockModels;
            _renderLayers = renderLayers;
            _pool = pool;
            _entityModels = entityModels;
            _ubs = new UniformBufferSet(pool);
            _entityGbs = new GeometryBufferSet(pool);
        }
        
        public void QueueChunkUpdate(IChunk chunk)
        {
            _updatedChunks.Add(chunk);
        }

        public void QueueChunkRemoval(IChunk chunk)
        {
            _removedChunks.Add(chunk);
        }

        public void AddEntity(EntityInstance entity)
        {
            _entities.Add(entity.Id, entity);
        }

        public void RemoveEntity(Guid guid)
        {
            _entities.Remove(guid);
        }

        public void ReRender(bool queueRenderedChunks)
        {
            if (queueRenderedChunks)
                foreach (var chunk in _chunkRenderData.Keys)
                    QueueChunkUpdate(chunk);

            foreach (var data in _chunkRenderData.Values)
                data.Dispose();
            _chunkRenderData.Clear();
        }

        public void UpdateChunks()
        {
            if (_updatedChunks.Count == 0)
                return;

            foreach (var chunk in _removedChunks)
            {
                _chunkRenderData.Remove(chunk);
            }
            
            foreach (var chunk in _updatedChunks)
            {
                if (!_chunkRenderData.TryGetValue(chunk, out var renderData))
                    _chunkRenderData[chunk] = renderData = new ChunkRenderData(chunk, _blockModels, _pool);
                renderData.UpdateGeometry();
            }

            _removedChunks.Clear();
            _updatedChunks.Clear();
        }

        public void SubmitGeometry(RenderContext context, CommandBufferRecorder cmd, Matrix4x4 projection, ICamera camera, ViewFrustum viewFrustum)
        {
            _ubs.Clear();
            _ubs.Setup(context, cmd);

            var rendered = new List<(IChunk, ChunkRenderData)>();
            foreach (var (chunk, renderData) in _chunkRenderData)
            {
                var min = chunk.Position.GetOrigin();
                var max = min + Vector3.One * 16;

                if (!viewFrustum.Test(new AABB(min, max)))
                    continue;

                rendered.Add((chunk, renderData));
                renderData.UpdateDynamicGeometry();
            }

            _entityGbs.Clear();
            foreach (var layer in _renderLayers)
            {
                foreach (var entity in _entities.Values)
                {
                    if (!_entityModels.TryGetValue(entity.Type, out var model))
                        continue;

                    _entityGbs.Transform = Matrix4x4.Identity;
                    model.AddGeometry(entity, _entityGbs);

                    if (!model.HasDynamicGeometry)
                        continue;

                    _entityGbs.Transform = Matrix4x4.Identity;
                    model.AddDynamicGeometry(entity, _entityGbs);
                }
            }

            foreach (var layer in _renderLayers)
            {
                foreach (var (chunk, renderData) in rendered)
                {
                    var transform = Matrix4x4.CreateTranslation(chunk.Position.GetOrigin()) * camera.Transform * projection;
                    _ubs.AddAndUse(context, cmd, layer, transform);
                    renderData.SubmitGeometry(context, layer, cmd);
                }

                _ubs.AddAndUse(context, cmd, layer, camera.Transform * projection);
                _entityGbs.Draw(layer, context, cmd);
            }

            _ubs.Finalize(context, cmd);
        }
    }
}