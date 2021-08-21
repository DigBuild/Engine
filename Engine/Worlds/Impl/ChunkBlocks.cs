using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Math;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worlds.Impl
{
    public interface IReadOnlyChunkBlocks : IEnumerable<KeyValuePair<ChunkBlockPos, Block?>>, IChangeNotifier
    {
        public Block? GetBlock(ChunkBlockPos pos);
        internal DataContainer? GetData(ChunkBlockPos pos);

        public IEnumerable<KeyValuePair<ChunkBlockPos, Block>> EnumerateNonNull();
    }

    public class ChunkBlocks : IReadOnlyChunkBlocks, IData<ChunkBlocks>
    {
        private const uint ChunkSize = WorldDimensions.ChunkSize;
        
        public static DataHandle<IChunk, IReadOnlyChunkBlocks, ChunkBlocks> Type { get; internal set; } = null!;
        
        private readonly Octree<Block?>[] _blocks = new Octree<Block?>[WorldDimensions.ChunkVerticalSubdivisions];
        private readonly Dictionary<ChunkBlockPos, DataContainer> _data = new();

        public event Action? Changed;

        public ChunkBlocks()
        {
            for (var i = 0; i < _blocks.Length; i++)
                _blocks[i] = new Octree<Block?>(4, null);
        }

        private void NotifyChange()
        {
            Changed?.Invoke();
        }

        public Block? GetBlock(ChunkBlockPos pos) => _blocks[pos.Y >> 4][pos.X, pos.Y & 15, pos.Z];
        internal DataContainer? GetData(ChunkBlockPos pos) => _data.TryGetValue(pos, out var d) ? d : null;
        DataContainer? IReadOnlyChunkBlocks.GetData(ChunkBlockPos pos) => GetData(pos);

        public void SetBlock(ChunkBlockPos pos, Block? block)
        {
            if (_blocks[pos.Y >> 4][pos.X, pos.Y & 15, pos.Z] == block)
                return;
            _blocks[pos.Y >> 4][pos.X, pos.Y & 15, pos.Z] = block;
            var d = block?.CreateDataContainer();
            if (d != null)
            {
                _data.TryGetValue(pos, out var oldData);
                _data[pos] = d;

                d.Changed += NotifyChange;
                if (oldData != null)
                    oldData.Changed -= NotifyChange;
            }

            NotifyChange();
        }

        public IEnumerable<KeyValuePair<ChunkBlockPos, Block>> EnumerateNonNull()
        {
            for (var i = 0; i < _blocks.Length; i++)
            {
                var octree = _blocks[i];
                foreach (var ((x, y, z), block) in octree.EnumerateNonNull())
                {
                    yield return new KeyValuePair<ChunkBlockPos, Block>(new ChunkBlockPos(x, y + (i << 4), z), block!);
                }
            }
        }

        public IEnumerator<KeyValuePair<ChunkBlockPos, Block?>> GetEnumerator()
        {
            for (var i = 0; i < _blocks.Length; i++)
            {
                var octree = _blocks[i];
                foreach (var ((x, y, z), block) in octree)
                {
                    yield return new KeyValuePair<ChunkBlockPos, Block?>(new ChunkBlockPos(x, y + (i << 4), z), block);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ChunkBlocks Copy()
        {
            var copy = new ChunkBlocks();
            for (var i = 0; i < WorldDimensions.ChunkVerticalSubdivisions; i++)
            for (var x = 0; x < ChunkSize; x++) // TODO: Optimized octree copy
            for (var y = 0; y < ChunkSize; y++)
            for (var z = 0; z < ChunkSize; z++)
                copy._blocks[i][x, y, z] = _blocks[i][x, y, z];

            foreach (var (pos, d) in _data)
            {
                var newData = copy._data[pos] = d.Copy();
                newData.Changed += copy.NotifyChange;
            }

            return copy;
        }
        
        public static ISerdes<ChunkBlocks> Serdes { get; } = new SimpleSerdes<ChunkBlocks>(
            (stream, blocks) =>
            {
                var bw = new BinaryWriter(stream);
                
                for (var i = 0; i < WorldDimensions.ChunkVerticalSubdivisions; i++)
                for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                for (var z = 0; z < ChunkSize; z++)
                {
                    var block = blocks._blocks[i][x, y, z];

                    bw.Write(block != null);
                    if (block == null)
                        continue;

                    bw.Write(block.Name.ToString());

                    blocks._data.TryGetValue(new ChunkBlockPos(x, y, z), out var data);
                    
                    block.DataSerdes.Serialize(stream, data);
                }
            },
            (stream, context) =>
            {
                var br = new BinaryReader(stream);

                var blocks = new ChunkBlocks();
                
                for (var i = 0; i < WorldDimensions.ChunkVerticalSubdivisions; i++)
                for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                for (var z = 0; z < ChunkSize; z++)
                {
                    if (!br.ReadBoolean())
                        continue;

                    var name = ResourceName.Parse(br.ReadString())!;
                    var block = BuiltInRegistries.Blocks.GetOrNull(name.Value)!;
                    blocks._blocks[i][x, y, z] = block;
                    
                    var data = block.DataSerdes.Deserialize(stream, context);
                    if (data != null)
                    {
                        blocks._data[new ChunkBlockPos(x, y, z)] = data;
                        data.Changed += blocks.NotifyChange;
                    }
                }

                return blocks;
            });
    }

    public static class ChunkBlocksExtensions
    {
        public static Block? GetBlock(this IReadOnlyChunk chunk, ChunkBlockPos pos)
        {
            return chunk.Get(ChunkBlocks.Type).GetBlock(pos);
        }

        public static bool SetBlock(this IChunk chunk, ChunkBlockPos pos, Block? block)
        {
            chunk.Get(ChunkBlocks.Type).SetBlock(pos, block);
            return true;
        }
        
        public static Block? GetBlock(this IReadOnlyWorld world, BlockPos pos)
        {
            return world.GetChunk(pos.ChunkPos)?.GetBlock(pos.SubChunkPos);
        }

        public static bool SetBlock(this IWorld world, BlockPos pos, Block? block, bool notifyLeave = true, bool notifyJoin = true, bool reRender = true)
        {
            var storage = world.GetChunk(pos.ChunkPos)?.Get(ChunkBlocks.Type);
            if (storage == null)
                return false;

            if (notifyLeave)
            {
                var curBlock = storage.GetBlock(pos.SubChunkPos);
                curBlock?.OnLeavingWorld(world, pos);
            }

            storage.SetBlock(pos.SubChunkPos, block);

            if (notifyJoin && block != null)
            {
                block.OnJoinedWorld(world, pos);
            }

            world.OnBlockChanged(pos);
            if (reRender)
                world.MarkBlockForReRender(pos);
            return true;
        }
    }
}