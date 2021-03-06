using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public abstract class WorldBase : IWorld
    {
        IChunk? IWorld.GetChunk(ChunkPos pos, bool load)
        {
            return GetChunk(pos, load);
        }
        public abstract IChunk? GetChunk(ChunkPos pos, bool load = true);

        public virtual Block? GetBlock(BlockPos pos)
        {
            return GetChunk(pos.ChunkPos)?.BlockStorage.Blocks[pos.X & 15, pos.Y & 15, pos.Z & 15];
        }

        public BlockDataContainer? GetData(BlockPos pos)
        {
            return GetChunk(pos.ChunkPos)?.BlockStorage.Data[pos.X & 15, pos.Y & 15, pos.Z & 15];
        }

        public virtual void SetBlock(BlockPos pos, Block? block)
        {
            var storage = GetChunk(pos.ChunkPos)!.BlockStorage;
            storage.Blocks[pos.X & 15, pos.Y & 15, pos.Z & 15] = block;
            storage.Data[pos.X & 15, pos.Y & 15, pos.Z & 15] = new BlockDataContainer();
        }
    }
}