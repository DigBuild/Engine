using System;
using System.Collections.Generic;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public sealed class Region : IRegion, IDisposable
    {
        public const ulong TemporaryChunkExpirationDelay = 20;
        public const ulong LowDensityRegionExpirationDelay = 20;

        private readonly IRegionStorage _storage;
        private readonly IChunkProvider _chunkProvider;

        private readonly Cache<RegionChunkPos, Chunk> _chunks;
        private readonly Dictionary<RegionChunkPos, HashSet<ChunkClaim>> _loadingClaims = new();
        private readonly LockStore<RegionChunkPos> _locks = new();

        private readonly IndividualCache<ILowDensityRegion> _lowDensity;

        private readonly DataContainer<IRegion> _data;

        public RegionPos Position { get; }

        public ILowDensityRegion LowDensity
        {
            get
            {
                lock (_lowDensity)
                {
                    if (_lowDensity.HasValue)
                        return _lowDensity.Value;
                    return _lowDensity.Value = _storage.LoadOrCreateManagedLowDensity();
                }
            }
        }

        public event Action<IChunk>? ChunkLoaded;
        public event Action<IChunk>? ChunkUnloaded;

        public Region(RegionPos position, IRegionStorage storage, IChunkProvider chunkProvider, ITickSource tickSource)
        {
            Position = position;
            _storage = storage;
            _chunkProvider = chunkProvider;
            _chunks = new Cache<RegionChunkPos, Chunk>(tickSource, TemporaryChunkExpirationDelay);
            _lowDensity = new IndividualCache<ILowDensityRegion>(tickSource, LowDensityRegionExpirationDelay);
            _data = _storage.LoadOrCreateManagedData();
            _chunks.EntryEvicted += (_, chunk) => ChunkUnloaded?.Invoke(chunk);
        }

        public void Dispose()
        {
            _chunks.Dispose();
            _lowDensity.Dispose();
        }

        internal bool HasClaims => _loadingClaims.Count > 0;

        internal void AddClaim(List<RegionChunkPos> positions, bool immediate, ChunkClaim claim)
        {
            foreach (var pos in positions)
            {
                if (!_loadingClaims.TryGetValue(pos, out var claims))
                {
                    _loadingClaims[pos] = claims = new HashSet<ChunkClaim>();
                    bool couldPersist;
                    lock (_chunks)
                    {
                        couldPersist = _chunks.Persist(pos);
                    }
                    if (!couldPersist && immediate)
                        Get(pos);
                }
                claims.Add(claim);
            }
        }

        internal void RemoveClaim(List<RegionChunkPos> positions, ChunkClaim claim)
        {
            foreach (var pos in positions)
            {
                if (!_loadingClaims.TryGetValue(pos, out var claims))
                    continue;

                claims.Remove(claim);
                if (claims.Count != 0)
                    continue;

                _loadingClaims.Remove(pos);
                lock (_chunks)
                {
                    _chunks.UnPersist(pos);
                }
            }
        }

        public bool IsLoaded(RegionChunkPos pos)
        {
            lock (_chunks)
            {
                return _chunks.ContainsKey(pos);
            }
        }

        public IChunk? Get(RegionChunkPos pos, bool loadOrGenerate = true)
        {
            using var lck = _locks.Lock(pos);

            Chunk? chunk;
            lock (_chunks)
            {
                if (_chunks.TryGetValue(pos, out chunk))
                    return chunk;
            }
            if (!loadOrGenerate)
                return null;
            var loaded = _storage.TryLoad(pos, out chunk);
            if (loaded || _chunkProvider.TryGet(Position + pos, out chunk))
            {
                chunk!.Changed += () =>
                {
                    _storage.Save(chunk);
                };
                lock (_chunks)
                {
                    _chunks.Add(pos, chunk, _loadingClaims.ContainsKey(pos));
                }
                if (!loaded)
                    _storage.Save(chunk);
                ChunkLoaded?.Invoke(chunk);
                return chunk;
            }
            return null;
        }

        public T Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            return _data.Get(type);
        }
    }
}