using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuildEngine.Voxel;
using DigBuildPlatformCS.Render;
using DigBuildPlatformCS.Util;

namespace DigBuildEngine.Render
{
    public class ChunkRenderData : IDisposable
    {
        private readonly Chunk _chunk;
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly GeometryBufferSet _gbs;

        public ChunkRenderData(Chunk chunk, IReadOnlyDictionary<Block, IBlockModel> blockModels, NativeBufferPool pool)
        {
            _chunk = chunk;
            _blockModels = blockModels;
            _gbs = new GeometryBufferSet(pool);
        }

        public void UpdateGeometry()
        {
            _gbs.Clear();

            for (var x = 0; x < 16; x++)
            {
                for (var y = 0; y < 16; y++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        var block = _chunk.BlockStorage.Blocks[x, y, z];
                        if (block == null || !_blockModels.TryGetValue(block, out var model))
                            continue;

                        var faces = BlockFaceFlags.None;
                        if (x == 0 || (!_chunk.BlockStorage.Blocks[x - 1, y, z]?.Solid ?? true))
                            faces |= BlockFaceFlags.NegX;
                        if (x == 15 || (!_chunk.BlockStorage.Blocks[x + 1, y, z]?.Solid ?? true))
                            faces |= BlockFaceFlags.PosX;
                        if (y == 0 || (!_chunk.BlockStorage.Blocks[x, y - 1, z]?.Solid ?? true))
                            faces |= BlockFaceFlags.NegY;
                        if (y == 15 || (!_chunk.BlockStorage.Blocks[x, y + 1, z]?.Solid ?? true))
                            faces |= BlockFaceFlags.PosY;
                        if (z == 0 || (!_chunk.BlockStorage.Blocks[x, y, z - 1]?.Solid ?? true))
                            faces |= BlockFaceFlags.NegZ;
                        if (z == 15 || (!_chunk.BlockStorage.Blocks[x, y, z + 1]?.Solid ?? true))
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