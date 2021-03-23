using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyWorld
    {
        public ulong AbsoluteTime { get; }
        
        public TReadOnly Get<TReadOnly, T>(WorldStorageType<TReadOnly, T> type)
            where TReadOnly : IReadOnlyWorldStorage
            where T : TReadOnly, IWorldStorage<T>;

        IReadOnlyChunk? GetChunk(ChunkPos pos, bool load = true);
    }
}