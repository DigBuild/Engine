using System.Collections.Generic;
using DigBuildEngine.Voxel;
using DigBuildPlatformCS.Render;

namespace DigBuildEngine.Render
{
    public class WorldRenderManager
    {
        private readonly HashSet<Chunk> _updatedChunks = new();
        private readonly Dictionary<Chunk, ChunkRenderData> _chunkRenderData = new();
        private readonly ICamera _camera;

        public WorldRenderManager(ICamera camera)
        {
            _camera = camera;
        }

        public void QueueChunkUpdate(Chunk chunk)
        {
            _updatedChunks.Add(chunk);
        }

        public void Reset(bool queueRenderedChunks)
        {
            if (queueRenderedChunks)
            {
                foreach (var chunk in _chunkRenderData.Keys)
                {
                    QueueChunkUpdate(chunk);
                }
            }

            _chunkRenderData.Clear();
        }

        public void UpdateChunks(RenderContext renderContext)
        {
            if (_updatedChunks.Count == 0)
                return;
            
            foreach (var chunk in _updatedChunks)
            {
                if (!_chunkRenderData.TryGetValue(chunk, out var renderData))
                    _chunkRenderData[chunk] = renderData = new ChunkRenderData(renderContext, chunk);
                renderData.UpdateGeometry();
            }

            _updatedChunks.Clear();
        }

        public void SubmitGeometry(CommandBufferRecorder cmd, float aspectRatio, float partialTick)
        {
            if (_chunkRenderData.Count > 0)
                ChunkRenderData.BeginSubmit(cmd, aspectRatio);
            foreach (var renderData in _chunkRenderData.Values)
                renderData.SubmitGeometry(_camera, cmd, partialTick);
            if (_chunkRenderData.Count > 0)
                ChunkRenderData.CompleteSubmit();
        }
    }
}