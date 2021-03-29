using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Worldgen
{
    public class WorldGenerator
    {
        private readonly IReadOnlyCollection<IWorldgenFeature> _features;
        private readonly long _seed;
        private readonly Func<WorldSliceDescriptor, int> _heightGetter;

        public WorldGenerator(IReadOnlyCollection<IWorldgenFeature> features, long seed, Func<WorldSliceDescriptor, int> heightGetter)
        {
            _features = features;
            _seed = seed;
            _heightGetter = heightGetter;
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

        public IReadOnlyChunk[] GenerateSlice(WorldSlicePos pos)
        {
            WorldSliceDescriptionContext descriptionContext = new(pos, _seed);
            foreach (var feature in _features)
            {
                descriptionContext.NeighborDescriptor = p => DescribeSlice(p, feature);
                feature.DescribeSlice(descriptionContext);
                descriptionContext.Next();
            }
            
            var descriptor = descriptionContext.CreateDescriptor();
            var height = _heightGetter(descriptor);
            var prototypes = new IReadOnlyChunk[height];
            Parallel.For(0, height, y =>
            {
                var prototype = new ChunkPrototype(new ChunkPos(pos.X, y, pos.Z));
                foreach (var feature in _features)
                    feature.PopulateChunk(descriptor, prototype);
                prototypes[y] = prototype;
            });

            return prototypes;
        }
    }
}