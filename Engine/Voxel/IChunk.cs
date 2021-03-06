using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public interface IChunk
    {
        public ChunkPos Position { get; }
        public IBlockChunkStorage BlockStorage { get; }

        // public T GetStorage<T>() where T : IChunkStorage;
    }
    
    public interface IChunkStorage
    {
    }
}