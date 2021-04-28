using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Math;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Impl.Worlds
{
    public interface IReadOnlyChunkBlocks : IEnumerable<KeyValuePair<ChunkBlockPosition, Block?>>
    {
        public Block? GetBlock(ChunkBlockPosition pos);
        internal DataContainer? GetData(ChunkBlockPosition pos);
    }

    public class ChunkBlocks : IReadOnlyChunkBlocks, IData<ChunkBlocks>, IChangeNotifier
    {
        private const uint ChunkSize = 16;
        
        public static DataHandle<IChunk, IReadOnlyChunkBlocks, ChunkBlocks> Type { get; internal set; } = null!;
        
        private readonly Octree<Block?> _blocks = new(4, null);
        private readonly Octree<DataContainer?> _data = new(4, null);

        public event Action? Changed;

        public Block? GetBlock(ChunkBlockPosition pos) => _blocks[pos.X, pos.Y, pos.Z];
        internal DataContainer? GetData(ChunkBlockPosition pos) => _data[pos.X, pos.Y, pos.Z];
        DataContainer? IReadOnlyChunkBlocks.GetData(ChunkBlockPosition pos) => GetData(pos);

        public void SetBlock(ChunkBlockPosition pos, Block? block)
        {
            if (_blocks[pos.X, pos.Y, pos.Z] == block)
                return;
            _blocks[pos.X, pos.Y, pos.Z] = block;
            _data[pos.X, pos.Y, pos.Z] = block?.CreateDataContainer();
            Changed?.Invoke();
        }

        public IEnumerator<KeyValuePair<ChunkBlockPosition, Block?>> GetEnumerator()
        {
            foreach (var ((x, y, z), block) in _blocks)
            {
                yield return new KeyValuePair<ChunkBlockPosition, Block?>(new ChunkBlockPosition(x, y, z), block);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ChunkBlocks Copy()
        {
            var copy = new ChunkBlocks();
            for (var x = 0; x < ChunkSize; x++)
            for (var y = 0; y < ChunkSize; y++)
            for (var z = 0; z < ChunkSize; z++)
            {
                copy._blocks[x, y, z] = _blocks[x, y, z];
                copy._data[x, y, z] = _data[x, y, z]?.Copy();
            }
            return copy;
        }

        private static readonly ISerdes<DataContainer?> NullableDataContainerSerdes = new NullableSerdes<DataContainer>(DataContainer.Serdes);
        public static ISerdes<ChunkBlocks> Serdes { get; } = new SimpleSerdes<ChunkBlocks>(
            (stream, blocks) =>
            {
                var bw = new BinaryWriter(stream);

                for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                for (var z = 0; z < ChunkSize; z++)
                {
                    var block = blocks._blocks[x, y, z];

                    bw.Write(block != null);
                    if (block == null)
                        continue;

                    bw.Write(block.Name.ToString());

                    var data = blocks._data[x, y, z];
                    NullableDataContainerSerdes.Serialize(stream, data);
                }
            },
            stream =>
            {
                var br = new BinaryReader(stream);

                var blocks = new ChunkBlocks();

                for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                for (var z = 0; z < ChunkSize; z++)
                {
                    if (!br.ReadBoolean())
                        continue;

                    var name = ResourceName.Parse(br.ReadString())!;
                    var block = BuiltInRegistries.Blocks.GetOrNull(name.Value)!;
                    blocks._blocks[x, y, z] = block;
                    
                    var data = NullableDataContainerSerdes.Deserialize(stream);
                    blocks._data[x, y, z] = data;
                }

                return blocks;
            });
    }

    public static class ChunkBlocksExtensions
    {
        public static Block? GetBlock(this IReadOnlyChunk chunk, ChunkBlockPosition pos)
        {
            return chunk.Get(ChunkBlocks.Type).GetBlock(pos);
        }

        public static bool SetBlock(this IChunk chunk, ChunkBlockPosition pos, Block? block)
        {
            chunk.Get(ChunkBlocks.Type).SetBlock(pos, block);
            return true;
        }
        
        public static Block? GetBlock(this IReadOnlyWorld world, BlockPos pos)
        {
            return world.GetChunk(pos.ChunkPos)?.GetBlock(pos.SubChunkPos);
        }

        public static bool SetBlock(this IWorld world, BlockPos pos, Block? block, bool notifyLeave = true, bool notifyJoin = true)
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
            return true;
        }
    }
}