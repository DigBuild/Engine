using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;
using DigBuild.Engine.Voxel;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class WorldRenderManager
    {
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly IEnumerable<IWorldRenderLayer> _renderLayers;
        private readonly NativeBufferPool _pool;
        private readonly UniformBufferSet _ubs;

        private readonly HashSet<IChunk> _updatedChunks = new();
        private readonly Dictionary<IChunk, ChunkRenderData> _chunkRenderData = new();

        public WorldRenderManager(IReadOnlyDictionary<Block, IBlockModel> blockModels, IEnumerable<IWorldRenderLayer> renderLayers, NativeBufferPool pool)
        {
            _blockModels = blockModels;
            _renderLayers = renderLayers;
            _pool = pool;
            _ubs = new UniformBufferSet(pool);
        }

        public void QueueChunkUpdate(IChunk chunk)
        {
            _updatedChunks.Add(chunk);
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
            
            foreach (var chunk in _updatedChunks)
            {
                if (!_chunkRenderData.TryGetValue(chunk, out var renderData))
                    _chunkRenderData[chunk] = renderData = new ChunkRenderData(chunk, _blockModels, _pool);
                renderData.UpdateGeometry();
            }

            _updatedChunks.Clear();
        }

        public void SubmitGeometry(RenderContext context, CommandBufferRecorder cmd, ICamera camera, ViewFrustum viewFrustum)
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

            foreach (var layer in _renderLayers)
            {
                foreach (var (chunk, renderData) in rendered)
                {
                    var transform = Matrix4x4.CreateTranslation(chunk.Position.GetOrigin()) * camera.Transform;
                    _ubs.AddAndUse(context, cmd, layer, transform);
                    renderData.SubmitGeometry(context, layer, cmd);
                }
            }

            _ubs.Finalize(context, cmd);
        }
    }
}