using DigBuildEngine.Math;

namespace DigBuildEngine.Voxel
{
    public interface IWorld
    {
        // public T GetStorage<T>() where T : IWorldStorage<T> => throw new NotImplementedException();
        
        IChunk? GetChunk(ChunkPos pos, bool load = true);

        Block? GetBlock(BlockPos pos);

        void SetBlock(BlockPos pos, Block? block);
    }

    // public interface IWorldStorage<T> where T : IWorldStorage<T>
    // {
    //
    // }
}