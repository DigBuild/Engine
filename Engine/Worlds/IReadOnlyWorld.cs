using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    /// <summary>
    /// A read-only world.
    /// </summary>
    public interface IReadOnlyWorld
    {
        /// <summary>
        /// The absolute time.
        /// </summary>
        ulong AbsoluteTime { get; }

        /// <summary>
        /// The force of gravity.
        /// </summary>
        float Gravity { get; }
        
        /// <summary>
        /// Gets a read-only data handle.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only handle type</typeparam>
        /// <typeparam name="T">The read-write handle type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        TReadOnly Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier;

        /// <summary>
        /// Gets the chunk at the given position, optionally loading it if it's not already.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="load">Whether to load the chunk if not loaded</param>
        /// <returns>The chunk, or null if missing</returns>
        IReadOnlyChunk? GetChunk(ChunkPos pos, bool load = true);
    }
}