using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class ChunkRenderData : IDisposable
    {
        private readonly IChunk _chunk;
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly GeometryBufferSet _gbs;

        private readonly List<DynamicModelData> _dynamicModelData = new();
        private readonly GeometryBufferSet _gbsDynamic;

        public ChunkRenderData(IChunk chunk, IReadOnlyDictionary<Block, IBlockModel> blockModels, NativeBufferPool pool)
        {
            _chunk = chunk;
            _blockModels = blockModels;
            _gbs = new GeometryBufferSet(pool);
            _gbsDynamic = new GeometryBufferSet(pool);
        }

        public void UpdateGeometry()
        {
            _dynamicModelData.Clear();
            _gbs.Clear();

            var storage = _chunk.Get(BlockChunkStorage.Type);
            for (var x = 0; x < 16; x++)
            {
                for (var y = 0; y < 16; y++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        var block = storage.GetBlock(new SubChunkPos(x, y, z));
                        if (block == null || !_blockModels.TryGetValue(block, out var model))
                            continue;

                        var faces = DirectionFlags.None;
                        if (x == 0 || storage.GetBlock(new SubChunkPos(x - 1, y, z)) == null)
                            faces |= DirectionFlags.NegX;
                        if (x == 15 || storage.GetBlock(new SubChunkPos(x + 1, y, z)) == null)
                            faces |= DirectionFlags.PosX;
                        if (y == 0 || storage.GetBlock(new SubChunkPos(x, y - 1, z)) == null)
                            faces |= DirectionFlags.NegY;
                        if (y == 15 || storage.GetBlock(new SubChunkPos(x, y + 1, z)) == null)
                            faces |= DirectionFlags.PosY;
                        if (z == 0 || storage.GetBlock(new SubChunkPos(x, y, z - 1)) == null)
                            faces |= DirectionFlags.NegZ;
                        if (z == 15 || storage.GetBlock(new SubChunkPos(x, y, z + 1)) == null)
                            faces |= DirectionFlags.PosZ;

                        _gbs.Transform = Matrix4x4.CreateTranslation(x, y, z);
                        model.AddGeometry(faces, _gbs);
                        if (model.HasDynamicGeometry)
                            _dynamicModelData.Add(new DynamicModelData(x, y, z, model));
                    }
                }
            }
        }

        public void UpdateDynamicGeometry()
        {
            if (_dynamicModelData.Count == 0)
                return;

            _gbsDynamic.Clear();
            
            foreach (var data in _dynamicModelData)
            {
                _gbsDynamic.Transform = Matrix4x4.CreateTranslation((Vector3) data.Position);
                data.Model.AddDynamicGeometry(_gbsDynamic);
            }
        }

        public void SubmitGeometry(RenderContext context, IRenderLayer layer, CommandBufferRecorder cmd)
        {
            _gbs.Draw(layer, context, cmd);
            if (_dynamicModelData.Count > 0)
                _gbsDynamic.Draw(layer, context, cmd);
        }

        public void Dispose()
        {
            _gbs.Dispose();
            _gbsDynamic.Dispose();
        }

        private sealed class DynamicModelData
        {
            internal readonly Vector3I Position;
            internal readonly IBlockModel Model;

            public DynamicModelData(int x, int y, int z, IBlockModel model)
            {
                Position = new Vector3I(x, y, z);
                Model = model;
            }
        }
    }
}