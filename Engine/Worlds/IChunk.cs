namespace DigBuild.Engine.Worlds
{
    public interface IChunk : IReadOnlyChunk
    {
        public new T Get<TReadOnly, T>(ChunkStorageType<TReadOnly, T> type)
            where TReadOnly : IReadOnlyChunkStorage
            where T : TReadOnly, IChunkStorage<T>;
        TReadOnly IReadOnlyChunk.Get<TReadOnly, T>(ChunkStorageType<TReadOnly, T> type) => Get(type);

        public void CopyFrom(IReadOnlyChunk other);
    }
}