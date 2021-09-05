using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds.Impl
{
    internal sealed class ChunkLoadingClaim : IChunkLoadingClaim
    {
        private readonly Action<ChunkLoadingClaim> _release;

        public IEnumerable<ChunkPos> Chunks { get; }

        public ChunkLoadingClaim(ImmutableHashSet<ChunkPos> chunks, Action<ChunkLoadingClaim> release)
        {
            Chunks = chunks;
            _release = release;
        }

        public void Release()
        {
            _release(this);
        }
    }
}