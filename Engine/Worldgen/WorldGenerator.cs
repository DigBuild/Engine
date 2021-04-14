using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;
using DigBuild.Engine.Utils;

namespace DigBuild.Engine.Worldgen
{
    public class WorldGenerator : IChunkProvider
    {
        public const ulong DescriptorExpirationDelay = 20;

        private readonly Cache<WorldSlicePos, WorldSliceDescriptor> _sliceDescriptors;
        private readonly IReadOnlyCollection<IWorldgenFeature> _features;
        private readonly long _seed;

        public WorldGenerator(ITickSource tickSource, IReadOnlyCollection<IWorldgenFeature> features, long seed)
        {
            _sliceDescriptors = new Cache<WorldSlicePos, WorldSliceDescriptor>(tickSource, DescriptorExpirationDelay);
            _features = features;
            _seed = seed;
        }

        public WorldSliceDescriptor DescribeSlice(WorldSlicePos pos, IWorldgenFeature? stop)
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

        public Chunk GenerateChunk(ChunkPos pos)
        {
            WorldSliceDescriptor? descriptor;
            lock (_sliceDescriptors)
            {
                var slicePos = new WorldSlicePos(pos.X, pos.Z);
                if (!_sliceDescriptors.TryGetValue(slicePos, out descriptor))
                    _sliceDescriptors[slicePos] = descriptor = DescribeSlice(slicePos, null);
            }

            var chunk = new Chunk(pos);
            foreach (var feature in _features)
                feature.PopulateChunk(descriptor, chunk);
            return chunk;
        }

        public bool TryGet(ChunkPos pos, [NotNullWhen(true)] out Chunk? chunk)
        {
            chunk = GenerateChunk(pos);
            return true;
        }
    }
}