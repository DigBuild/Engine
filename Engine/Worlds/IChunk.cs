using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    public interface IChunk : IReadOnlyChunk, IChangeNotifier
    {
        public const uint Size = WorldDimensions.ChunkSize;
        
        new T Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        
        TReadOnly IReadOnlyChunk.Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> type) => Get(type);
    }
}