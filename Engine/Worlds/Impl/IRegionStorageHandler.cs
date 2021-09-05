using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A storage handler for world regions.
    /// </summary>
    public interface IRegionStorageHandler
    {
        /// <summary>
        /// Tries to load a chunk in this region from disk.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="chunk">The chunk</param>
        /// <returns>Whether it was loaded or not</returns>
        bool TryLoad(RegionChunkPos pos, [NotNullWhen(true)] out Chunk? chunk);

        /// <summary>
        /// Queues up a chunk for saving.
        /// </summary>
        /// <param name="chunk">The chunk</param>
        void Save(Chunk chunk);

        /// <summary>
        /// Loads or instantiates a managed data container that should auto-save on changes.
        /// </summary>
        /// <returns>The data container</returns>
        DataContainer<IRegion> LoadOrCreateManagedData();
    }
}