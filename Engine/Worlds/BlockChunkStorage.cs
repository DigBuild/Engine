using DigBuild.Engine.Blocks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyBlockChunkStorage : IReadOnlyChunkStorage
    {
        public Block? GetBlock(SubChunkPos pos);
        internal DataContainer? GetData(SubChunkPos pos);
    }

    public class BlockChunkStorage : IReadOnlyBlockChunkStorage, IChunkStorage<BlockChunkStorage>
    {
        private const uint ChunkSize = 16;
        
        public static ChunkStorageType<IReadOnlyBlockChunkStorage, BlockChunkStorage> Type { get; internal set; } = null!;
        
        private readonly Block?[,,] _blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
        private readonly DataContainer?[,,] _data = new DataContainer[ChunkSize, ChunkSize, ChunkSize];

        public Block? GetBlock(SubChunkPos pos) => _blocks[pos.X, pos.Y, pos.Z];
        internal DataContainer? GetData(SubChunkPos pos) => _data[pos.X, pos.Y, pos.Z];
        DataContainer? IReadOnlyBlockChunkStorage.GetData(SubChunkPos pos) => GetData(pos);

        public void SetBlock(SubChunkPos pos, Block? block)
        {
            if (_blocks[pos.X, pos.Y, pos.Z] == block)
                return;
            _blocks[pos.X, pos.Y, pos.Z] = block;
            _data[pos.X, pos.Y, pos.Z] = block?.CreateDataContainer();
        }

        public BlockChunkStorage Copy()
        {
            var copy = new BlockChunkStorage();
            for (var x = 0; x < ChunkSize; x++)
            for (var y = 0; y < ChunkSize; y++)
            for (var z = 0; z < ChunkSize; z++)
            {
                copy._blocks[x, y, z] = _blocks[x, y, z];
                copy._data[x, y, z] = _data[x, y, z]?.Copy();
            }
            return copy;
        }
    }

    public static class BlockChunkStorageWorldExtensions
    {
        public static Block? GetBlock(this IReadOnlyChunk chunk, BlockPos pos)
        {
            return chunk.Get(BlockChunkStorage.Type).GetBlock(pos.SubChunkPos);
        }

        public static bool SetBlock(this IChunk chunk, BlockPos pos, Block? block)
        {
            chunk.Get(BlockChunkStorage.Type).SetBlock(pos.SubChunkPos, block);
            return true;
        }
        
        public static Block? GetBlock(this IReadOnlyWorld world, BlockPos pos)
        {
            return world.GetChunk(pos.ChunkPos)?.GetBlock(pos);
        }

        public static bool SetBlock(this IWorld world, BlockPos pos, Block? block, bool notifyLeave = true, bool notifyJoin = true)
        {
            var storage = world.GetChunk(pos.ChunkPos)?.Get(BlockChunkStorage.Type);
            if (storage == null)
                return false;

            if (notifyLeave)
            {
                var curBlock = storage.GetBlock(pos.SubChunkPos);
                curBlock?.OnLeavingWorld(new BlockContext(world, pos, curBlock), new BuiltInBlockEvent.LeavingWorld());
            }

            storage.SetBlock(pos.SubChunkPos, block);

            if (notifyJoin && block != null)
            {
                block.OnJoinedWorld(new BlockContext(world, pos, block), new BuiltInBlockEvent.JoinedWorld());
            }

            world.OnBlockChanged(pos);
            return true;
        }
    }
}