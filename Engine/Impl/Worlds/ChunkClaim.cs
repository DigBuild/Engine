﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    internal sealed class ChunkClaim : IChunkClaim
    {
        private readonly Action<ChunkClaim> _release;

        public IEnumerable<ChunkPos> Chunks { get; }

        public ChunkClaim(ImmutableHashSet<ChunkPos> chunks, Action<ChunkClaim> release)
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