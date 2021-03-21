using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public interface IReadOnlyBlockChunkStorage : IReadOnlyChunkStorage
    {
        public IBlocks Blocks { get; }
        public IData Data { get; }
        
        public interface IBlocks
        {
            public Block? this[int x, int y, int z] { get; }
        }
        public interface IData
        {
            public BlockDataContainer? this[int x, int y, int z] { get; }
        }
    }

    public class BlockChunkStorage : IReadOnlyBlockChunkStorage, IChunkStorage<BlockChunkStorage>
    {
        private const uint ChunkSize = 16;

        public BlockContainer Blocks { get; } = new();
        public BlockDataContainerContainer Data { get; } = new();

        IReadOnlyBlockChunkStorage.IBlocks IReadOnlyBlockChunkStorage.Blocks => Blocks;
        IReadOnlyBlockChunkStorage.IData IReadOnlyBlockChunkStorage.Data => Data;
        
        public BlockChunkStorage Copy()
        {
            var copy = new BlockChunkStorage();
            for (var x = 0; x < ChunkSize; x++)
            for (var y = 0; y < ChunkSize; y++)
            for (var z = 0; z < ChunkSize; z++)
            {
                copy.Blocks[x, y, z] = Blocks[x, y, z];
                copy.Data[x, y, z] = Data[x, y, z];
            }

            return copy;
        }

        public sealed class BlockContainer : IReadOnlyBlockChunkStorage.IBlocks
        {
            private readonly Block?[,,] _blocks = new Block[ChunkSize, ChunkSize, ChunkSize];

            public Block? this[int x, int y, int z]
            {
                get => _blocks[x, y, z];
                set => _blocks[x, y, z] = value;
            }
        }

        public sealed class BlockDataContainerContainer : IReadOnlyBlockChunkStorage.IData
        {
            private readonly BlockDataContainer?[,,] _data = new BlockDataContainer[ChunkSize, ChunkSize, ChunkSize];
            
            public BlockDataContainer? this[int x, int y, int z]
            {
                get => _data[x, y, z];
                set => _data[x, y, z] = value;
            }
        }
    }

    public static class BlockChunkStorageWorldExtensions
    {
        public static Block? GetBlock(this IReadOnlyChunk chunk, BlockPos pos)
        {
            return chunk.Get<BlockChunkStorage>().Blocks[pos.X & 15, pos.Y & 15, pos.Z & 15];
        }
        public static BlockDataContainer? GetData(this IReadOnlyChunk chunk, BlockPos pos)
        {
            return chunk.Get<BlockChunkStorage>().Data[pos.X & 15, pos.Y & 15, pos.Z & 15];
        }
        public static bool SetBlock(this IChunk chunk, BlockPos pos, Block? block)
        {
            var storage = chunk.Get<BlockChunkStorage>();
            if (storage.Blocks[pos.X & 15, pos.Y & 15, pos.Z & 15] == block)
                return false;

            storage.Blocks[pos.X & 15, pos.Y & 15, pos.Z & 15] = block;
            storage.Data[pos.X & 15, pos.Y & 15, pos.Z & 15] = block?.CreateDataContainer(new BlockContext(null!, pos, block)); // TODO: FIX
            return true;
        }
        
        public static Block? GetBlock(this IReadOnlyWorld world, BlockPos pos)
        {
            return world.GetChunk(pos.ChunkPos)?.GetBlock(pos);
        }
        public static BlockDataContainer? GetData(this IReadOnlyWorld world, BlockPos pos)
        {
            return world.GetChunk(pos.ChunkPos)?.GetData(pos);
        }
        public static bool SetBlock(this IWorld world, BlockPos pos, Block? block)
        {
            var chunk = world.GetChunk(pos.ChunkPos);
            if (chunk == null)
                return false;

            chunk.SetBlock(pos, block);
            world.OnBlockChanged(pos);
            return true;
        }
    }
}