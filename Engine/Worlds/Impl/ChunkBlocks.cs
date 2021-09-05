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
    /// <summary>
    /// A read-only view of the blocks in a chunk.
    /// </summary>
    public interface IReadOnlyChunkBlocks : IEnumerable<KeyValuePair<ChunkBlockPos, Block?>>, IChangeNotifier
    {
        /// <summary>
        /// Gets the block at a given position.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <returns>The block, or null if none</returns>
        Block? GetBlock(ChunkBlockPos pos);
        internal DataContainer? GetData(ChunkBlockPos pos);

        /// <summary>
        /// Enumerates all the non-null blocks in the chunk.
        /// </summary>
        /// <returns>The enumeration</returns>
        IEnumerable<KeyValuePair<ChunkBlockPos, Block>> EnumerateNonNull();
    }

    /// <summary>
    /// A chunk data type for block storage.
    /// </summary>
    public class ChunkBlocks : IReadOnlyChunkBlocks, IData<ChunkBlocks>
    {
        private const uint ChunkWidth = WorldDimensions.ChunkWidth;
        
        /// <summary>
        /// The data handle for chunk blocks.
        /// </summary>
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

        /// <summary>
        /// Sets the block at a given position.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="block">The block</param>
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
            for (var x = 0; x < ChunkWidth; x++) // TODO: Optimized octree copy
            for (var y = 0; y < 16; y++)
            for (var z = 0; z < ChunkWidth; z++)
                copy._blocks[i][x, y, z] = _blocks[i][x, y, z];

            foreach (var (pos, d) in _data)
            {
                var newData = copy._data[pos] = d.Copy();
                newData.Changed += copy.NotifyChange;
            }

            return copy;
        }
        
        /// <summary>
        /// The serdes.
        /// </summary>
        public static ISerdes<ChunkBlocks> Serdes { get; } = new SimpleSerdes<ChunkBlocks>(
            (stream, blocks) =>
            {
                var bw = new BinaryWriter(stream);
                
                for (var i = 0; i < WorldDimensions.ChunkVerticalSubdivisions; i++)
                for (var x = 0; x < ChunkWidth; x++)
                for (var y = 0; y < ChunkWidth; y++)
                for (var z = 0; z < ChunkWidth; z++)
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
                for (var x = 0; x < ChunkWidth; x++)
                for (var y = 0; y < ChunkWidth; y++)
                for (var z = 0; z < ChunkWidth; z++)
                {
                    if (!br.ReadBoolean())
                        continue;

                    var name = ResourceName.Parse(br.ReadString())!;
                    var block = DigBuildEngine.Blocks.GetOrNull(name.Value)!;
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

    /// <summary>
    /// Helper methods for interfacing with blocks.
    /// </summary>
    public static class ChunkBlocksExtensions
    {
        /// <summary>
        /// Gets the block at a specific position within the chunk.
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <param name="pos">The position</param>
        /// <returns>The block, or null if none</returns>
        public static Block? GetBlock(this IReadOnlyChunk chunk, ChunkBlockPos pos)
        {
            return chunk.Get(ChunkBlocks.Type).GetBlock(pos);
        }

        /// <summary>
        /// Sets the block at a specific position within the chunk.
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <param name="pos">The position</param>
        /// <param name="block">The block</param>
        /// <returns>Whether the operation was successful or not</returns>
        public static bool SetBlock(this IChunk chunk, ChunkBlockPos pos, Block? block)
        {
            chunk.Get(ChunkBlocks.Type).SetBlock(pos, block);
            return true;
        }
        
        /// <summary>
        /// Gets the block at a specific position in the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        /// <returns>The block, or null if none</returns>
        public static Block? GetBlock(this IReadOnlyWorld world, BlockPos pos)
        {
            return world.GetChunk(pos.ChunkPos)?.GetBlock(pos.SubChunkPos);
        }

        /// <summary>
        /// Sets the block at a specific position in the world, optionally notifying different systems.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        /// <param name="block">The block</param>
        /// <param name="notifyLeave">Whether to notify the existing block that it's leaving</param>
        /// <param name="notifyJoin">Whether to notify the new block that it's joining</param>
        /// <param name="reRender">Whether to re-render the chunk</param>
        /// <returns>Whether the operation was successful or not</returns>
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