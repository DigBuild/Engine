using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyChunk
    {
        public ChunkPos Position { get; }
        
        public TReadOnly Get<TReadOnly, T>(ChunkStorageType<TReadOnly, T> type)
            where TReadOnly : IReadOnlyChunkStorage
            where T : TReadOnly, IChunkStorage<T>;
    }
}