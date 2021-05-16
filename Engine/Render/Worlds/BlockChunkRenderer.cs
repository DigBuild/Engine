using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Math;
using DigBuild.Engine.Render.Models;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render.Worlds
{
    public class BlockChunkRenderer : IChunkRenderer
    {
        private readonly IReadOnlyWorld _world;
        private readonly IReadOnlyChunk _chunk;
        private readonly IReadOnlyDictionary<Block, Models.IBlockModel> _blockModels;
        
        private readonly IReadOnlyChunkBlocks _blockStorage;
        private readonly IReadOnlyChunkBlockLight _lightingStorage;
        
        private readonly GeometryBuffer _geometryBuffer;
        private readonly GeometryBuffer _dynamicGeometryBuffer;
        private readonly List<DynamicModelData> _dynamicModelData = new();

        private bool _updated = true;

        public BlockChunkRenderer(
            IReadOnlyWorld world,
            IReadOnlyChunk chunk,
            IReadOnlyDictionary<Block, Models.IBlockModel> blockModels,
            NativeBufferPool bufferPool
        )
        {
            _world = world;
            _chunk = chunk;
            _blockModels = blockModels;
            
            _blockStorage = _chunk.Get(ChunkBlocks.Type);
            _lightingStorage = _chunk.Get(IChunkBlockLight.Type);

            _geometryBuffer = new GeometryBuffer(bufferPool);
            _dynamicGeometryBuffer = new GeometryBuffer(bufferPool);
            
            _blockStorage.Changed += OnChanged;
            _lightingStorage.Changed += OnChanged;
        }

        public void Dispose()
        {
            _blockStorage.Changed -= OnChanged;
            _lightingStorage.Changed -= OnChanged;
        }

        private void OnChanged()
        {
            _updated = true;
        }

        private void StaticUpdate(RenderContext context)
        {
            _dynamicModelData.Clear();

            var chunks = new Dictionary<ChunkPos, IReadOnlyChunk>
            {
                [_chunk.Position] = _chunk
            };
            foreach (var direction in Directions.All)
            {
                var pos = _chunk.Position.Offset(direction);
                var neighbor = _world.GetChunk(pos, false);
                if (neighbor != null)
                    chunks[pos] = neighbor;
            }

            var solidityCache = new Dictionary<ChunkBlockPos, BlockFaceSolidity>();
            bool IsNeighborFaceSolid(ChunkBlockPos pos, Direction direction)
            {
                var absPos = (_chunk.Position + pos).Offset(direction);
                var (chunkPos, blockPos) = absPos;
                var sameChunk = chunkPos == _chunk.Position;

                if (!sameChunk || !solidityCache!.TryGetValue(blockPos, out var solidity))
                {
                    if (chunks!.TryGetValue(chunkPos, out var chunk))
                    {
                        var block = chunk.GetBlock(blockPos);
                        solidity = block?.Get(new ReadOnlyBlockContext(_world, absPos, block), BlockFaceSolidity.Attribute) ?? BlockFaceSolidity.None;

                        if (sameChunk)
                            solidityCache![blockPos] = solidity;
                    }
                    else
                    {
                        solidity = BlockFaceSolidity.All;
                    }
                }

                return solidity.Solid.Has(direction.GetOpposite());
            }

            foreach (var (pos, block) in _blockStorage)
            {
                if (block == null || !_blockModels.TryGetValue(block, out var model))
                    continue;

                var modelData = block.Get(new ReadOnlyBlockContext(_world, _chunk.Position + pos, block), ModelData.BlockAttribute);
                var visibleFaces = Directions.All
                    .Where(direction => IsNeighborFaceSolid(pos, direction))
                    .Aggregate(DirectionFlags.All, (current, direction) => (DirectionFlags) (current - direction.ToFlags()));
                var lighting = 1f;//lightingStorage.Get(pos) / (float) 0xF;

                _geometryBuffer.Transform = Matrix4x4.CreateTranslation((Vector3) pos);
                model.AddGeometry(_geometryBuffer, modelData, visibleFaces);

                if (model.HasDynamicGeometry)
                    _dynamicModelData.Add(new DynamicModelData(pos, block, visibleFaces, lighting, model));
            }

            _geometryBuffer.Upload(context);
        }

        public void Update(RenderContext context, WorldView worldView, float partialTick)
        {
            if (_updated)
            {
                StaticUpdate(context);
                _updated = false;
            }

            if (_dynamicModelData.Count <= 0)
                return;

            _dynamicGeometryBuffer.Clear();
            foreach (var data in _dynamicModelData)
            {
                var modelData = data.Block.Get(new ReadOnlyBlockContext(_world, _chunk.Position + data.Pos, data.Block), ModelData.BlockAttribute);

                _geometryBuffer.Transform = Matrix4x4.CreateTranslation((Vector3) data.Pos);
                data.Model.AddDynamicGeometry(_geometryBuffer, modelData, data.VisibleFaces, partialTick);
            }
            _dynamicGeometryBuffer.Upload(context);
        }

        public void BeforeDraw(RenderContext context, CommandBufferRecorder cmd, UniformBufferSet uniforms,
            WorldView worldView, float partialTick)
        {
        }

        public void Draw(RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer,
            IReadOnlyUniformBufferSet uniforms, WorldView worldView, float partialTick)
        {
            _geometryBuffer.Draw(cmd, layer, uniforms);
            _dynamicGeometryBuffer.Draw(cmd, layer, uniforms);
        }

        public void AfterDraw(RenderContext context, CommandBufferRecorder cmd, WorldView worldView, float partialTick)
        {
        }
        
        private sealed class DynamicModelData
        {
            public readonly ChunkBlockPos Pos;
            public readonly Block Block;
            public readonly DirectionFlags VisibleFaces;
            public readonly float Lighting;
            public readonly Models.IBlockModel Model;

            public DynamicModelData(ChunkBlockPos pos, Block block, DirectionFlags visibleFaces, float lighting, Models.IBlockModel model)
            {
                Pos = pos;
                Block = block;
                VisibleFaces = visibleFaces;
                Lighting = lighting;
                Model = model;
            }
        }
    }
}