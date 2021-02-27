namespace DigBuildEngine.Voxel
{
    public interface IChunk
    {
        public BlockChunkStorage BlockStorage { get; }

        // public T GetStorage<T>() where T : IChunkStorage;
    }
    
    public interface IChunkStorage
    {
    }
}