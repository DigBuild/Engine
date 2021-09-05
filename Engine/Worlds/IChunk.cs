using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    /// <summary>
    /// A world chunk.
    /// </summary>
    public interface IChunk : IReadOnlyChunk, IChangeNotifier
    {
        /// <summary>
        /// The width of the chunk.
        /// </summary>
        public const uint Width = WorldDimensions.ChunkWidth;
        /// <summary>
        /// The height of the chunk.
        /// </summary>
        public const uint Height = WorldDimensions.ChunkHeight;
        
        /// <summary>
        /// Gets a read-write data handle.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only handle type</typeparam>
        /// <typeparam name="T">The read-write handle type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        new T Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        
        TReadOnly IReadOnlyChunk.Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> handle) => Get(handle);
    }
}