using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds
{
    /// <summary>
    /// A world.
    /// </summary>
    public interface IWorld : IReadOnlyWorld
    {
        /// <summary>
        /// The chunk manager.
        /// </summary>
        IChunkManager ChunkManager { get; }

        /// <summary>
        /// The tick scheduler.
        /// </summary>
        Scheduler TickScheduler { get; }
        
        /// <summary>
        /// Gets a read-write data handle.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only handle type</typeparam>
        /// <typeparam name="T">The read-write handle type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        new T Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        TReadOnly IReadOnlyWorld.Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> handle) => Get(handle);
        
        /// <summary>
        /// Gets the chunk at the given position, optionally loading/generating it if it's not already.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="loadOrGenerate">Whether to load/generate the chunk if not loaded</param>
        /// <returns>The chunk, or null if missing</returns>
        new IChunk? GetChunk(ChunkPos pos, bool loadOrGenerate = true);
        IReadOnlyChunk? IReadOnlyWorld.GetChunk(ChunkPos pos, bool loadOrGenerate) => GetChunk(pos, loadOrGenerate);

        /// <summary>
        /// Handles block changes.
        /// </summary>
        /// <param name="pos">The position</param>
        void OnBlockChanged(BlockPos pos);
        /// <summary>
        /// Handles entity addition.
        /// </summary>
        /// <param name="entity">The entity</param>
        void OnEntityAdded(EntityInstance entity);
        /// <summary>
        /// Handles entity removal.
        /// </summary>
        /// <param name="entity">The entity</param>
        void OnEntityRemoving(EntityInstance entity);

        /// <summary>
        /// Marks a chunk to be re-rendered.
        /// </summary>
        /// <param name="pos">The position</param>
        void MarkChunkForReRender(ChunkPos pos);
        /// <summary>
        /// Marks a block to be re-rendered.
        /// </summary>
        /// <param name="pos">The position</param>
        void MarkBlockForReRender(BlockPos pos);
    }
}