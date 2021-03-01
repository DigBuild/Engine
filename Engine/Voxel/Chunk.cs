using System;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public class Chunk : IChunk
    {
        public ChunkPos Position { get; }
        public BlockChunkStorage BlockStorage { get; }

        public Chunk(ChunkPos position, Action<Chunk> notifyUpdate)
        {
            Position = position;
            BlockStorage = new BlockChunkStorage(() => notifyUpdate(this));
        }
    }
}