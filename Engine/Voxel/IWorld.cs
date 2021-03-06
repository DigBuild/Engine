using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public interface IWorld : IReadOnlyWorld
    {
        // public T GetStorage<T>() where T : IWorldStorage<T> => throw new NotImplementedException();
        
        IChunk? GetChunk(ChunkPos pos, bool load = true);

        Block? GetBlock(BlockPos pos);

        BlockDataContainer? GetData(BlockPos pos);

        void SetBlock(BlockPos pos, Block? block);
    }

    // public interface IWorldStorage<T> where T : IWorldStorage<T>
    // {
    //
    // }
}