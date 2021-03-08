using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Worldgen
{
    public class WorldGenerator
    {
        private readonly IReadOnlyCollection<IWorldgenFeature> _features;
        private readonly long _seed;
        private readonly Func<ChunkPos, IChunk> _chunkPrototypeFactory;

        public WorldGenerator(IReadOnlyCollection<IWorldgenFeature> features, long seed, Func<ChunkPos, IChunk> chunkPrototypeFactory)
        {
            _features = features;
            _seed = seed;
            _chunkPrototypeFactory = chunkPrototypeFactory;
        }

        public WorldSliceDescriptor DescribeSlice(WorldSlicePos pos, IWorldgenFeature stop)
        {
            WorldSliceDescriptionContext descriptionContext = new(pos, _seed);
            foreach (var feature in _features)
            {
                if (feature == stop)
                    return descriptionContext.CreateDescriptor();
                
                descriptionContext.NeighborDescriptor = p => DescribeSlice(p, feature);
                feature.DescribeSlice(descriptionContext);
                descriptionContext.Next();
            }
            return descriptionContext.CreateDescriptor();
        }

        public void GenerateSlice(WorldSlicePos pos, IChunk[] chunks)
        {
            WorldSliceDescriptionContext descriptionContext = new(pos, _seed);
            foreach (var feature in _features)
            {
                descriptionContext.NeighborDescriptor = p => DescribeSlice(p, feature);
                feature.DescribeSlice(descriptionContext);
                descriptionContext.Next();
            }

            int height = chunks.Length;

            var descriptor = descriptionContext.CreateDescriptor();
            var chunkPrototypes = new IChunk[height];
            for (var y = 0; y < height; y++)
            {
                var prototype = chunkPrototypes[y] = _chunkPrototypeFactory(new ChunkPos(pos.X, y, pos.Z));
                foreach (var feature in _features) 
                    feature.PopulateChunk(descriptor, prototype);
            }
            
            for (var i = 0; i < height; i++)
                chunks[i].CopyFrom(chunkPrototypes[i]);
        }
    }
}