using DigBuildEngine.Math;
using DigBuildEngine.Worldgen;

namespace DigBuildEngine.Voxel
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