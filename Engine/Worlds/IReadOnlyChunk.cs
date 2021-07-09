using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyChunk
    {
        ChunkPos Position { get; }

        TReadOnly Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
    }
}