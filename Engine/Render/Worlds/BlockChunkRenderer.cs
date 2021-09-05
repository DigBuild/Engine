using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;
using DigBuild.Engine.Render.Models;
using DigBuild.Engine.Worlds;
using DigBuild.Engine.Worlds.Impl;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render.Worlds
{
    /// <summary>
    /// A chunk renderer that draws blocks.
    /// </summary>
    public class BlockChunkRenderer : IChunkRenderer
    {
        private readonly IReadOnlyWorld _world;
        private readonly IReadOnlyChunk _chunk;
        private readonly IReadOnlyDictionary<Block, IBlockModel> _blockModels;
        
        private readonly IReadOnlyChunkBlocks _blockStorage;
        private readonly IReadOnlyChunkBlockLight _lightingStorage;
        
        private readonly GeometryBuffer _geometryBuffer;
        private readonly GeometryBuffer _dynamicGeometryBuffer;
        private readonly List<DynamicModelData> _dynamicModelData = new();

        private bool _updated = true;
        private bool _uploadStatic = false;

        public BlockChunkRenderer(
            IReadOnlyWorld world,
            IReadOnlyChunk chunk,
            IReadOnlyDictionary<Block, IBlockModel> blockModels,
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

        /// <summary>
        /// Notifies the block renderer that it must update.
        /// </summary>
        public void OnChanged()
        {
            _updated = true;
        }

        private void StaticUpdate()
        {
            _dynamicModelData.Clear();
            _geometryBuffer.Clear();

            var chunks = new IReadOnlyChunk?[3, 3];
            chunks[1, 1] = _chunk;
            foreach (var direction in Directions.Horizontal)
            {
                var pos = _chunk.Position.Offset(direction);
                var neighbor = _world.GetChunk(pos, false);
                if (neighbor == null)
                    continue;
                var (offX, _, offZ) = direction.GetOffsetI() + Vector3I.One;
                chunks[offX, offZ] = neighbor;
            }

            const uint chunkSize = WorldDimensions.ChunkWidth;
            const uint chunkHeight = WorldDimensions.ChunkHeight;
            var solidityCache = new BlockFaceSolidity?[chunkSize, chunkHeight, chunkSize];
            bool IsNeighborFaceSolid(ChunkBlockPos pos, Direction direction)
            {
                var absPos = (_chunk.Position + pos).Offset(direction);
                if (absPos.OutOfBounds) return true;
                var (chunkPos, blockPos) = absPos;
                var (relX, relZ) = chunkPos - _chunk.Position;
                var sameChunk = relX == 0 && relZ == 0;

                BlockFaceSolidity? solidity;
                if (!sameChunk || (solidity = solidityCache[blockPos.X, blockPos.Y, blockPos.Z]) == null)
                {
                    var chunk = chunks[relX + 1, relZ + 1];
                    if (chunk != null)
                    {
                        var block = chunk.GetBlock(blockPos);
                        solidity = block?.Get(_world, absPos, BlockFaceSolidity.Attribute) ?? BlockFaceSolidity.None;

                        if (sameChunk)
                            solidityCache[blockPos.X, blockPos.Y, blockPos.Z] = solidity;
                    }
                    else
                    {
                        solidity = BlockFaceSolidity.All;
                    }
                }

                return solidity.Value.Solid.Has(direction.GetOpposite());
            }

            foreach (var (pos, block) in _blockStorage.EnumerateNonNull())
            {
                if (!_blockModels.TryGetValue(block, out var model))
                    continue;

                var modelData = block.Get(_world, _chunk.Position + pos, ModelData.BlockAttribute);
                var visibleFaces = Directions.All
                    .Where(direction => IsNeighborFaceSolid(pos, direction))
                    .Aggregate(DirectionFlags.All, (current, direction) => (DirectionFlags) (current - direction.ToFlags()));
                var lighting = 1f;//lightingStorage.Get(pos) / (float) 0xF;

                _geometryBuffer.Transform = Matrix4x4.CreateTranslation((Vector3) pos);
                model.AddGeometry(_geometryBuffer, modelData, visibleFaces);

                if (model.HasDynamicGeometry)
                    _dynamicModelData.Add(new DynamicModelData(pos, block, visibleFaces, lighting, model));
            }
        }

        public bool HasHeavyUpdate => _updated;

        public void Update(bool express, WorldView worldView, float partialTick)
        {
            if (!express && _updated)
            {
                StaticUpdate();
                _uploadStatic = true;
                _updated = false;
            }

            if (_dynamicModelData.Count <= 0)
                return;
            
            _dynamicGeometryBuffer.Clear();
            foreach (var data in _dynamicModelData)
            {
                var modelData = data.Block.Get(_world, _chunk.Position + data.Pos, ModelData.BlockAttribute);

                _dynamicGeometryBuffer.Transform = Matrix4x4.CreateTranslation((Vector3) data.Pos);
                data.Model.AddDynamicGeometry(_dynamicGeometryBuffer, modelData, data.VisibleFaces, partialTick);
            }
        }

        public void Upload(RenderContext context, WorldView worldView, float partialTick)
        {
            if (_uploadStatic)
            {
                _geometryBuffer.Upload(context);
                _uploadStatic = false;
            }

            if (_dynamicModelData.Count <= 0)
                return;
            
            _dynamicGeometryBuffer.Upload(context);
        }

        public void BeforeDraw(RenderContext context, CommandBufferRecorder cmd, UniformBufferSet uniforms,
            WorldView worldView, float partialTick)
        {
        }

        public void Draw(RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer, RenderLayerBindingSet bindings,
            IReadOnlyUniformBufferSet uniforms, WorldView worldView, float partialTick)
        {
            _geometryBuffer.Draw(cmd, layer, bindings, uniforms);
            if (_dynamicModelData.Count > 0)
                _dynamicGeometryBuffer.Draw(cmd, layer, bindings, uniforms);
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
            public readonly IBlockModel Model;

            public DynamicModelData(ChunkBlockPos pos, Block block, DirectionFlags visibleFaces, float lighting, IBlockModel model)
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