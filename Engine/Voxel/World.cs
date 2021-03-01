using DigBuild.Engine.Math;
using DigBuild.Engine.Worldgen;

namespace DigBuild.Engine.Voxel
{
    public class World : WorldBase
    {
        public ChunkManager ChunkManager { get; }

        public World(WorldGenerator generator)
        {
            ChunkManager = new ChunkManager(generator);
        }

        public override Chunk? GetChunk(ChunkPos pos, bool load = true)
        {
            return ChunkManager.Get(pos, load);
        }
    }
}