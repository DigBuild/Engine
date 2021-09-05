using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A read-only view of a region.
    /// </summary>
    public interface IReadOnlyRegion
    {
        /// <summary>
        /// The position.
        /// </summary>
        RegionPos Position { get; }

        /// <summary>
        /// Checks whether a chunk within the region is loaded or not.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <returns>Whether the chunk is loaded or not</returns>
        bool IsLoaded(RegionChunkPos pos);

        /// <summary>
        /// Tries to get the chunk at the given position, optionally loading or generating it.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="chunk">The chunk</param>
        /// <param name="loadOrGenerate">Whether to load or generate if missing</param>
        /// <returns>Whether the chunk was found/loaded/generated or not</returns>
        bool TryGet(RegionChunkPos pos, [NotNullWhen(true)] out IReadOnlyChunk? chunk, bool loadOrGenerate = true);
        
        /// <summary>
        /// Gets the read-only data for a handle.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only data type</typeparam>
        /// <typeparam name="T">The read-write data type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        TReadOnly Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier;
    }
}