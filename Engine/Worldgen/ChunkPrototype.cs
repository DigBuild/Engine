using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Worldgen
{
    public sealed class ChunkPrototype : ChunkBase
    {
        public override ChunkPos Position { get; }

        internal ChunkPrototype(ChunkPos position)
        {
            Position = position;
        }
    }
}