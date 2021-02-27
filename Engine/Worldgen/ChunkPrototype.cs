using System;
using DigBuildEngine.Math;
using DigBuildEngine.Voxel;

namespace DigBuildEngine.Worldgen
{
    public sealed class ChunkPrototype : IChunk
    {
        public ChunkPos Position { get; }
        public BlockChunkStorage BlockStorage { get; } = new(() => { });

        internal ChunkPrototype(ChunkPos position)
        {
            Position = position;
        }
    }
}