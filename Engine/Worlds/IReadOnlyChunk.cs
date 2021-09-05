using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    /// <summary>
    /// A read-only world chunk.
    /// </summary>
    public interface IReadOnlyChunk
    {
        /// <summary>
        /// The chunk position.
        /// </summary>
        ChunkPos Position { get; }
        
        /// <summary>
        /// Gets a read-only data handle.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only handle type</typeparam>
        /// <typeparam name="T">The read-write handle type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        TReadOnly Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier;
    }
}