﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Events;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A chunk manager that subdivides the world into regions.
    /// </summary>
    public class RegionManager : IChunkManager, IDisposable
    {
        private const ulong TemporaryRegionExpirationDelay = 50;

        private readonly Cache<RegionPos, Region> _regions;
        private readonly LockStore<RegionPos> _locks = new();

        private readonly IWorld _world;
        private readonly IChunkProvider _chunkProvider;
        private readonly Func<IWorld, RegionPos, IRegionStorageHandler> _storageProvider;
        private readonly ITickSource _tickSource;
        private readonly EventBus _eventBus;

        private readonly ProgressiveChunkLoader _chunkLoader;

        public RegionManager(
            IWorld world,
            IChunkProvider chunkProvider,
            Func<IWorld, RegionPos, IRegionStorageHandler> storageProvider,
            ITickSource tickSource,
            EventBus eventBus
        )
        {
            _regions = new Cache<RegionPos, Region>(tickSource, TemporaryRegionExpirationDelay);
            _world = world;
            _chunkProvider = chunkProvider;
            _tickSource = tickSource;
            _eventBus = eventBus;
            _storageProvider = storageProvider;
            _regions.EntryEvicted += (_, region) => region.Dispose();
            _chunkLoader = new ProgressiveChunkLoader(pos =>
            {
                var (regionPos, chunkPos) = pos;
                if (TryGet(regionPos, out var region))
                    region.TryGet(chunkPos, out _, true);
            });
        }

        public void Dispose()
        {
            _chunkLoader.Dispose();
            lock (_regions)
            {
                foreach (var (_, region) in _regions)
                    region.Dispose();
                _regions.Dispose();
            }
        }

        /// <summary>
        /// Tries to get the region at the given position.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="region">The region</param>
        /// <returns>Whether the region was found or not</returns>
        public bool TryGet(RegionPos pos, [NotNullWhen(true)] out Region? region)
        {
            using var lck = _locks.Lock(pos);

            lock (_regions)
            {
                if (_regions.TryGetValue(pos, out region))
                    return true;
            }

            region = new Region(pos, _storageProvider(_world, pos), _chunkProvider, _tickSource);
            region.ChunkLoaded += chunk => _eventBus.Post(new BuiltInChunkEvent.Loaded(_world, chunk));
            region.ChunkUnloaded += chunk => _eventBus.Post(new BuiltInChunkEvent.Unloading(_world, chunk));
            lock (_regions)
            {
                _regions[pos] = region;
            }
            return true;
        }
        
        public bool TryClaim(IEnumerable<ChunkPos> chunks, bool loadImmediately, out IChunkLoadingClaim claim)
        {
            var c = new ChunkLoadingClaim(chunks.ToImmutableHashSet(), Release);
            claim = c;

            var regionData = new Dictionary<RegionPos, (Region Region, List<RegionChunkPos> Chunks)>();

            foreach (var chunk in claim.Chunks)
            {
                var (regionPos, regionChunkPos) = chunk;

                if (regionData.TryGetValue(regionPos, out var data))
                {
                    data.Chunks.Add(regionChunkPos);
                    continue;
                }

                if (!TryGet(chunk.RegionPos, out var region))
                    return false;

                regionData[regionPos] = (region, new List<RegionChunkPos> {regionChunkPos});
            }

            foreach (var (region, regionChunks) in regionData.Values)
            {
                _regions.Persist(region.Position);
                region.AddClaim(regionChunks, loadImmediately, c);
            }
            
            if (!loadImmediately)
                _chunkLoader.Request(c.Chunks);
            
            return true;
        }

        private void Release(ChunkLoadingClaim claim)
        {
            var regionData = new Dictionary<RegionPos, (Region Region, List<RegionChunkPos> Chunks)>();

            foreach (var chunk in claim.Chunks)
            {
                var (regionPos, regionChunkPos) = chunk;

                if (regionData.TryGetValue(regionPos, out var data))
                {
                    data.Chunks.Add(regionChunkPos);
                    continue;
                }

                if (!TryGet(chunk.RegionPos, out var region))
                    throw new Exception();

                regionData[regionPos] = (region, new List<RegionChunkPos> {regionChunkPos});
            }
            
            foreach (var (region, regionChunks) in regionData.Values)
            {
                region.RemoveClaim(regionChunks, claim);
                if (!region.HasClaims)
                    _regions.UnPersist(region.Position);
            }
        }
    }
}