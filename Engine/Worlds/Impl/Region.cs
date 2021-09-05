using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A basic region implementation.
    /// </summary>
    public sealed class Region : IRegion, IDisposable
    {
        public const ulong TemporaryChunkExpirationDelay = 20;

        private readonly IRegionStorageHandler _storageHandler;
        private readonly IChunkProvider _chunkProvider;

        private readonly Cache<RegionChunkPos, Chunk> _chunks;
        private readonly Dictionary<RegionChunkPos, HashSet<ChunkLoadingClaim>> _loadingClaims = new();
        private readonly LockStore<RegionChunkPos> _locks = new();

        private readonly DataContainer<IRegion> _data;

        public RegionPos Position { get; }

        /// <summary>
        /// Fired when a chunk is loaded.
        /// </summary>
        public event Action<IChunk>? ChunkLoaded;
        /// <summary>
        /// Fired when a chunk is unloaded.
        /// </summary>
        public event Action<IChunk>? ChunkUnloaded;

        public Region(RegionPos position, IRegionStorageHandler storageHandler, IChunkProvider chunkProvider, ITickSource tickSource)
        {
            Position = position;
            _storageHandler = storageHandler;
            _chunkProvider = chunkProvider;
            _chunks = new Cache<RegionChunkPos, Chunk>(tickSource, TemporaryChunkExpirationDelay);
            _data = _storageHandler.LoadOrCreateManagedData();
            _chunks.EntryEvicted += (_, chunk) => ChunkUnloaded?.Invoke(chunk);
        }

        public void Dispose()
        {
            _chunks.Dispose();
        }

        internal bool HasClaims => _loadingClaims.Count > 0;

        internal void AddClaim(List<RegionChunkPos> positions, bool immediate, ChunkLoadingClaim claim)
        {
            foreach (var pos in positions)
            {
                if (!_loadingClaims.TryGetValue(pos, out var claims))
                {
                    _loadingClaims[pos] = claims = new HashSet<ChunkLoadingClaim>();
                    bool couldPersist;
                    lock (_chunks)
                    {
                        couldPersist = _chunks.Persist(pos);
                    }
                    if (!couldPersist && immediate)
                        TryGet(pos, out _);
                }
                claims.Add(claim);
            }
        }

        internal void RemoveClaim(List<RegionChunkPos> positions, ChunkLoadingClaim claim)
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

        public bool TryGet(RegionChunkPos pos, [MaybeNullWhen(false)] out IChunk? chunk, bool loadOrGenerate = true)
        {
            using var lck = _locks.Lock(pos);

            chunk = null;

            Chunk? c;
            lock (_chunks)
            {
                if (_chunks.TryGetValue(pos, out c))
                {
                    chunk = c;
                    return true;
                }
            }
            if (!loadOrGenerate)
                return false;
            var loaded = _storageHandler.TryLoad(pos, out c);
            if (loaded || _chunkProvider.TryGet(Position + pos, out c))
            {
                c!.Changed += () =>
                {
                    _storageHandler.Save(c);
                };
                lock (_chunks)
                {
                    _chunks.Add(pos, c, _loadingClaims.ContainsKey(pos));
                }
                if (!loaded)
                    _storageHandler.Save(c);
                ChunkLoaded?.Invoke(c);
                chunk = c;
                return true;
            }
            return false;
        }

        public T Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            return _data.Get(handle);
        }
    }
}