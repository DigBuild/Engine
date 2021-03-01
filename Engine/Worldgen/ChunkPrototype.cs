using DigBuild.Engine.Math;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Worldgen
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