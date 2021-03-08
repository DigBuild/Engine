using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Voxel;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public class ChunkRenderData : IDisposable
    {
        private readonly IChunk _chunk;
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly GeometryBufferSet _gbs;

        public ChunkRenderData(IChunk chunk, IReadOnlyDictionary<Block, IBlockModel> blockModels, NativeBufferPool pool)
        {
            _chunk = chunk;
            _blockModels = blockModels;
            _gbs = new GeometryBufferSet(pool);
        }

        public void UpdateGeometry()
        {
            _gbs.Clear();

            var storage = _chunk.Get<BlockChunkStorage>();
            for (var x = 0; x < 16; x++)
            {
                for (var y = 0; y < 16; y++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        var block = storage.Blocks[x, y, z];
                        if (block == null || !_blockModels.TryGetValue(block, out var model))
                            continue;

                        var faces = BlockFaceFlags.None;
                        if (x == 0 || storage.Blocks[x - 1, y, z] == null)
                            faces |= BlockFaceFlags.NegX;
                        if (x == 15 || storage.Blocks[x + 1, y, z] == null)
                            faces |= BlockFaceFlags.PosX;
                        if (y == 0 || storage.Blocks[x, y - 1, z] == null)
                            faces |= BlockFaceFlags.NegY;
                        if (y == 15 || storage.Blocks[x, y + 1, z] == null)
                            faces |= BlockFaceFlags.PosY;
                        if (z == 0 || storage.Blocks[x, y, z - 1] == null)
                            faces |= BlockFaceFlags.NegZ;
                        if (z == 15 || storage.Blocks[x, y, z + 1] == null)
                            faces |= BlockFaceFlags.PosZ;

                        _gbs.SetTransform(Matrix4x4.CreateTranslation(x, y, z));
                        model.AddGeometry(faces, _gbs);
                    }
                }
            }
        }

        public void SubmitGeometry(RenderContext context, IWorldRenderLayer layer, CommandBufferRecorder cmd)
        {
            _gbs.Draw(layer, context, cmd);
        }

        public void Dispose()
        {
            _gbs.Dispose();
        }
    }
}