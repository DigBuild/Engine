namespace DigBuild.Engine.Worlds
{
    public interface IChunkStorage : IReadOnlyChunkStorage
    {
        public IChunkStorage Copy();
    }

    public interface IChunkStorage<out T> : IChunkStorage where T : IChunkStorage<T>
    {
        public new T Copy();
        IChunkStorage IChunkStorage.Copy() => Copy();
    }
}