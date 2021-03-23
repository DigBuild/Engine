using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyChunk
    {
        public ChunkPos Position { get; }
        
        public T Get<T>() where T : class, IChunkStorage<T>, new();
    }
    
    public interface IReadOnlyChunkStorage
    {
    }
}