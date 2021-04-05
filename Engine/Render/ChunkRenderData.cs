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
        private readonly IReadOnlyChunk _chunk;
        private readonly Func<int, int, int, IReadOnlyChunk?> _neighborGetter;
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly GeometryBufferSet _gbs;

        private readonly List<DynamicModelData> _dynamicModelData = new();
        private readonly GeometryBufferSet _gbsDynamic;

        public ChunkRenderData(IReadOnlyChunk chunk, Func<int, int, int, IReadOnlyChunk?> neighborGetter, IReadOnlyDictionary<Block, IBlockModel> blockModels, NativeBufferPool pool)
        {
            _chunk = chunk;
            _neighborGetter = neighborGetter;
            _blockModels = blockModels;
            _gbs = new GeometryBufferSet(pool);
            _gbsDynamic = new GeometryBufferSet(pool);
        }

        public void UpdateGeometry()
        {
            _dynamicModelData.Clear();
            _gbs.Clear();
            
            var models = new IBlockModel?[18, 18, 18];
            var lighting = new byte[18, 18, 18];

            {
                var blockStorage = _chunk.Get(BlockChunkStorage.Type);
                var lightingStorage = _chunk.Get(IBlockLightStorage.Type);
                for (var x = 0; x < 16; x++)
                for (var y = 0; y < 16; y++)
                for (var z = 0; z < 16; z++)
                {
                    var subChunkPos = new SubChunkPos(x, y, z);
                    var block = blockStorage.GetBlock(subChunkPos);
                    if (block != null && _blockModels.TryGetValue(block, out var model))
                        models[x + 1, y + 1, z + 1] = model;
                    lighting[x + 1, y + 1, z + 1] = lightingStorage.Get(subChunkPos);
                }
            }
            
            {
                var negXNeighbor = _neighborGetter(-1, 0, 0);
                if (negXNeighbor != null)
                {
                    var blockStorage = negXNeighbor.Get(BlockChunkStorage.Type);
                    var lightingStorage = negXNeighbor.Get(IBlockLightStorage.Type);
                    for (var y = 0; y < 16; y++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new SubChunkPos(15, y, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[0, y + 1, z + 1] = model;
                        lighting[0, y + 1, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
            }
            {
                var posXNeighbor = _neighborGetter(1, 0, 0);
                if (posXNeighbor != null)
                {
                    var blockStorage = posXNeighbor.Get(BlockChunkStorage.Type);
                    var lightingStorage = posXNeighbor.Get(IBlockLightStorage.Type);
                    for (var y = 0; y < 16; y++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new SubChunkPos(0, y, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[17, y + 1, z + 1] = model;
                        lighting[17, y + 1, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
            }
            
            {
                var negYNeighbor = _neighborGetter(0, -1, 0);
                if (negYNeighbor != null)
                {
                    var blockStorage = negYNeighbor.Get(BlockChunkStorage.Type);
                    var lightingStorage = negYNeighbor.Get(IBlockLightStorage.Type);
                    for (var x = 0; x < 16; x++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new SubChunkPos(x, 15, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, 0, z + 1] = model;
                        lighting[x + 1, 0, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
            }
            {
                var posYNeighbor = _neighborGetter(0, 1, 0);
                if (posYNeighbor != null)
                {
                    var blockStorage = posYNeighbor.Get(BlockChunkStorage.Type);
                    var lightingStorage = posYNeighbor.Get(IBlockLightStorage.Type);
                    for (var x = 0; x < 16; x++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new SubChunkPos(x, 0, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, 17, z + 1] = model;
                        lighting[x + 1, 17, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
            }

            {
                var negZNeighbor = _neighborGetter(0, 0, -1);
                if (negZNeighbor != null)
                {
                    var blockStorage = negZNeighbor.Get(BlockChunkStorage.Type);
                    var lightingStorage = negZNeighbor.Get(IBlockLightStorage.Type);
                    for (var x = 0; x < 16; x++)
                    for (var y = 0; y < 16; y++)
                    {
                        var subChunkPos = new SubChunkPos(x, y, 15);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, y + 1, 0] = model;
                        lighting[x + 1, y + 1, 0] = lightingStorage.Get(subChunkPos);
                    }
                }
            }

            {
                var posZNeighbor = _neighborGetter(0, 0, 1);
                if (posZNeighbor != null)
                {
                    var blockStorage = posZNeighbor.Get(BlockChunkStorage.Type);
                    var lightingStorage = posZNeighbor.Get(IBlockLightStorage.Type);
                    for (var x = 0; x < 16; x++)
                    for (var y = 0; y < 16; y++)
                    {
                        var subChunkPos = new SubChunkPos(x, y, 0);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, y + 1, 17] = model;
                        lighting[x + 1, y + 1, 17] = lightingStorage.Get(subChunkPos);
                    }
                }
            }
            
            for (var x = 1; x < 17; x++)
            for (var y = 1; y < 17; y++)
            for (var z = 1; z < 17; z++)
            {
                var model = models[x, y, z];
                if (model == null)
                    continue;

                var faces = DirectionFlags.None;
                if (!(models[x - 1, y, z]?.IsFaceSolid(Direction.NegX) ?? false))
                    faces |= DirectionFlags.NegX;
                if (!(models[x + 1, y, z]?.IsFaceSolid(Direction.PosX) ?? false))
                    faces |= DirectionFlags.PosX;
                if (!(models[x, y - 1, z]?.IsFaceSolid(Direction.NegY) ?? false))
                    faces |= DirectionFlags.NegY;
                if (!(models[x, y + 1, z]?.IsFaceSolid(Direction.PosY) ?? false))
                    faces |= DirectionFlags.PosY;
                if (!(models[x, y, z - 1]?.IsFaceSolid(Direction.NegZ) ?? false))
                    faces |= DirectionFlags.NegZ;
                if (!(models[x, y, z + 1]?.IsFaceSolid(Direction.PosZ) ?? false))
                    faces |= DirectionFlags.PosZ;

                _gbs.Transform = Matrix4x4.CreateTranslation(x - 1, y - 1, z - 1);
                model.AddGeometry(faces, _gbs, dir =>
                {
                    var (offX, offY, offZ) = dir.GetOffsetI();
                    return lighting[x + offX, y + offY, z + offZ];
                });
                if (model.HasDynamicGeometry)
                    _dynamicModelData.Add(new DynamicModelData(x - 1, y - 1, z - 1, model));
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
                data.Model.AddDynamicGeometry(_gbsDynamic, _ => 0xF);
            }
        }

        public bool HasGeometry(IRenderLayer layer)
        {
            return _gbs.HasGeometry(layer) || (_dynamicModelData.Count > 0 && _gbsDynamic.HasGeometry(layer));
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