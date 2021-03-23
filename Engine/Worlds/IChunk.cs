namespace DigBuild.Engine.Worlds
{
    public interface IChunk : IReadOnlyChunk
    {
        // public new T Get<T>() where T : class, IChunkStorage, new();

        public void CopyFrom(IReadOnlyChunk other);
    }

    public interface IChunkStorage : IReadOnlyChunkStorage
    {
        public IChunkStorage Copy();
    }
    public interface IChunkStorage<out T> : IChunkStorage where T : class, IChunkStorage<T>, new()
    {
        public new T Copy();
        IChunkStorage IChunkStorage.Copy() => Copy();
    }
}