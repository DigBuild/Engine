﻿using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A world region.
    /// </summary>
    public interface IRegion : IReadOnlyRegion
    {
        public const uint Size = WorldDimensions.RegionSize;
        
        /// <summary>
        /// Tries to get the chunk at the given position, optionally loading or generating it.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="chunk">The chunk</param>
        /// <param name="loadOrGenerate">Whether to load or generate if missing</param>
        /// <returns>Whether the chunk was found/loaded/generated or not</returns>
        bool TryGet(RegionChunkPos pos, [MaybeNullWhen(false)] out IChunk? chunk, bool loadOrGenerate = true);
        
        /// <summary>
        /// Gets the read-write data for a handle.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only data type</typeparam>
        /// <typeparam name="T">The read-write data type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        new T Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        
        bool IReadOnlyRegion.TryGet(RegionChunkPos pos, [NotNullWhen(true)] out IReadOnlyChunk? chunk, bool loadOrGenerate)
        {
            return TryGet(pos, out chunk, loadOrGenerate);
        }
        TReadOnly IReadOnlyRegion.Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> handle) => Get(handle);
    }
}