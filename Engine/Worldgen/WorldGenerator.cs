using System.Collections.Generic;
using DigBuildEngine.Math;
using DigBuildEngine.Voxel;

namespace DigBuildEngine.Worldgen
{
    public class WorldGenerator
    {
        private const uint ChunkSize = 16;

        private readonly IReadOnlyCollection<IWorldgenFeature> _features;
        private readonly long _seed;

        public WorldGenerator(IReadOnlyCollection<IWorldgenFeature> features, long seed)
        {
            _features = features;
            _seed = seed;
        }

        public void GenerateSlice(WorldSlicePos pos, Chunk[] chunks)
        {
            WorldSliceDescriptionContext descriptionContext = new(pos, _seed);
            foreach (var feature in _features)
            {
                feature.DescribeSlice(descriptionContext);
                descriptionContext.Next();
            }

            int height = chunks.Length;

            var descriptor = descriptionContext.CreateDescriptor();
            var chunkPrototypes = new ChunkPrototype[height];
            for (var y = 0; y < height; y++)
            {
                var prototype = chunkPrototypes[y] = new ChunkPrototype(new ChunkPos(pos.X, y, pos.Z));
                foreach (var feature in _features)
                {
                    feature.PopulateChunk(descriptor, prototype);
                }
            }
            
            for (var i = 0; i < height; i++)
            {
                var prototype = chunkPrototypes[i];
                var chunk = chunks[i];
                for (var x = 0; x < ChunkSize; x++)
                {
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            chunk.BlockStorage.Blocks[x, y, z] = prototype.BlockStorage.Blocks[x, y, z];
                        }
                    }
                }
            }
        }
    }
}