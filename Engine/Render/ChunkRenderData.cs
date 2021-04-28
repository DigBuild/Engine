using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class ChunkRenderData : IDisposable
    {
        private readonly IReadOnlyWorld _world;
        private readonly IReadOnlyChunk _chunk;
        private readonly Func<int, int, int, IReadOnlyChunk?> _neighborGetter;
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        private readonly GeometryBufferSet _gbs;

        private readonly List<DynamicModelData> _dynamicModelData = new();
        private readonly GeometryBufferSet _gbsDynamic;

        public ChunkRenderData(IReadOnlyWorld world, IReadOnlyChunk chunk, Func<int, int, int, IReadOnlyChunk?> neighborGetter, IReadOnlyDictionary<Block, IBlockModel> blockModels, NativeBufferPool pool)
        {
            _world = world;
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
            
            var blocks = new Block[16, 16, 16];
            var data = new ModelData[16, 16, 16];
            var models = new IBlockModel?[18, 18, 18];
            var lighting = new byte[18, 18, 18];

            {
                var blockStorage = _chunk.Get(ChunkBlocks.Type);
                var lightingStorage = _chunk.Get(IChunkBlockLight.Type);
                for (var x = 0; x < 16; x++)
                for (var y = 0; y < 16; y++)
                for (var z = 0; z < 16; z++)
                {
                    var subChunkPos = new ChunkBlockPosition(x, y, z);
                    var block = blockStorage.GetBlock(subChunkPos);
                    if (block != null && _blockModels.TryGetValue(block, out var model))
                    {
                        blocks[x, y, z] = block;
                        data[x, y, z] = block.Get(new ReadOnlyBlockContext(_world, _chunk.Position + subChunkPos, block), ModelData.BlockAttribute);
                        models[x + 1, y + 1, z + 1] = model;
                    }
                    lighting[x + 1, y + 1, z + 1] = lightingStorage.Get(subChunkPos);
                }
            }
            
            {
                var negXNeighbor = _neighborGetter(-1, 0, 0);
                if (negXNeighbor != null)
                {
                    var blockStorage = negXNeighbor.Get(ChunkBlocks.Type);
                    var lightingStorage = negXNeighbor.Get(IChunkBlockLight.Type);
                    for (var y = 0; y < 16; y++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new ChunkBlockPosition(15, y, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[0, y + 1, z + 1] = model;
                        lighting[0, y + 1, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
                else
                {
                    for (var y = 0; y < 16; y++)
                    for (var z = 0; z < 16; z++)
                        models[0, y + 1, z + 1] = SolidBlockModel.Instance;
                }
            }
            {
                var posXNeighbor = _neighborGetter(1, 0, 0);
                if (posXNeighbor != null)
                {
                    var blockStorage = posXNeighbor.Get(ChunkBlocks.Type);
                    var lightingStorage = posXNeighbor.Get(IChunkBlockLight.Type);
                    for (var y = 0; y < 16; y++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new ChunkBlockPosition(0, y, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[17, y + 1, z + 1] = model;
                        lighting[17, y + 1, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
                else
                {
                    for (var y = 0; y < 16; y++)
                    for (var z = 0; z < 16; z++)
                        models[17, y + 1, z + 1] = SolidBlockModel.Instance;
                }
            }
            
            {
                var negYNeighbor = _neighborGetter(0, -1, 0);
                if (negYNeighbor != null)
                {
                    var blockStorage = negYNeighbor.Get(ChunkBlocks.Type);
                    var lightingStorage = negYNeighbor.Get(IChunkBlockLight.Type);
                    for (var x = 0; x < 16; x++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new ChunkBlockPosition(x, 15, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, 0, z + 1] = model;
                        lighting[x + 1, 0, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
                else
                {
                    for (var x = 0; x < 16; x++)
                    for (var z = 0; z < 16; z++)
                        models[x + 1, 0, z + 1] = SolidBlockModel.Instance;
                }
            }
            {
                var posYNeighbor = _neighborGetter(0, 1, 0);
                if (posYNeighbor != null)
                {
                    var blockStorage = posYNeighbor.Get(ChunkBlocks.Type);
                    var lightingStorage = posYNeighbor.Get(IChunkBlockLight.Type);
                    for (var x = 0; x < 16; x++)
                    for (var z = 0; z < 16; z++)
                    {
                        var subChunkPos = new ChunkBlockPosition(x, 0, z);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, 17, z + 1] = model;
                        lighting[x + 1, 17, z + 1] = lightingStorage.Get(subChunkPos);
                    }
                }
                else
                {
                    for (var x = 0; x < 16; x++)
                    for (var z = 0; z < 16; z++)
                        models[x + 1, 17, z + 1] = SolidBlockModel.Instance;
                }
            }

            {
                var negZNeighbor = _neighborGetter(0, 0, -1);
                if (negZNeighbor != null)
                {
                    var blockStorage = negZNeighbor.Get(ChunkBlocks.Type);
                    var lightingStorage = negZNeighbor.Get(IChunkBlockLight.Type);
                    for (var x = 0; x < 16; x++)
                    for (var y = 0; y < 16; y++)
                    {
                        var subChunkPos = new ChunkBlockPosition(x, y, 15);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, y + 1, 0] = model;
                        lighting[x + 1, y + 1, 0] = lightingStorage.Get(subChunkPos);
                    }
                }
                else
                {
                    for (var x = 0; x < 16; x++)
                    for (var y = 0; y < 16; y++)
                        models[x + 1, y + 1, 0] = SolidBlockModel.Instance;
                }
            }

            {
                var posZNeighbor = _neighborGetter(0, 0, 1);
                if (posZNeighbor != null)
                {
                    var blockStorage = posZNeighbor.Get(ChunkBlocks.Type);
                    var lightingStorage = posZNeighbor.Get(IChunkBlockLight.Type);
                    for (var x = 0; x < 16; x++)
                    for (var y = 0; y < 16; y++)
                    {
                        var subChunkPos = new ChunkBlockPosition(x, y, 0);
                        var block = blockStorage.GetBlock(subChunkPos);
                        if (block != null && _blockModels.TryGetValue(block, out var model))
                            models[x + 1, y + 1, 17] = model;
                        lighting[x + 1, y + 1, 17] = lightingStorage.Get(subChunkPos);
                    }
                }
                else
                {
                    for (var x = 0; x < 16; x++)
                    for (var y = 0; y < 16; y++)
                        models[x + 1, y + 1, 17] = SolidBlockModel.Instance;
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
                model.AddGeometry(_gbs, data[x - 1, y - 1, z - 1], dir =>
                {
                    var (offX, offY, offZ) = dir.GetOffsetI();
                    return lighting[x + offX, y + offY, z + offZ];
                }, faces);
                if (model.HasDynamicGeometry)
                    _dynamicModelData.Add(new DynamicModelData(x - 1, y - 1, z - 1, blocks[x - 1, y - 1, z - 1], model));
            }
        }

        public void UpdateDynamicGeometry(float partialTick)
        {
            if (_dynamicModelData.Count == 0)
                return;

            _gbsDynamic.Clear();
            
            foreach (var data in _dynamicModelData)
            {
                _gbsDynamic.Transform = Matrix4x4.CreateTranslation(new Vector3(data.Position.X, data.Position.Y, data.Position.Z));
                var modelData = data.Block.Get(
                    new ReadOnlyBlockContext(_world, _chunk.Position + data.Position, data.Block),
                    ModelData.BlockAttribute
                );
                data.Model.AddDynamicGeometry(_gbsDynamic, modelData, _ => 0xF, partialTick);
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
            internal readonly ChunkBlockPosition Position;
            internal readonly Block Block;
            internal readonly IBlockModel Model;

            public DynamicModelData(int x, int y, int z, Block block, IBlockModel model)
            {
                Position = new ChunkBlockPosition(x, y, z);
                Block = block;
                Model = model;
            }
        }
    }
}