using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;
using DigBuild.Engine.Worlds.Impl;

namespace DigBuild.Engine.Worldgen
{
    public class WorldGenerator : IChunkProvider
    {
        public const ulong DescriptorExpirationDelay = 20;

        private readonly Cache<ChunkPos, ChunkDescriptor> _sliceDescriptors;
        private readonly IReadOnlyCollection<IWorldgenFeature> _features;
        private readonly long _seed;

        public WorldGenerator(ITickSource tickSource, IReadOnlyCollection<IWorldgenFeature> features, long seed)
        {
            _sliceDescriptors = new Cache<ChunkPos, ChunkDescriptor>(tickSource, DescriptorExpirationDelay);
            _features = features;
            _seed = seed;
        }

        public ChunkDescriptor Describe(ChunkPos pos, IWorldgenFeature? stop)
        {
            ChunkDescriptionContext descriptionContext = new(pos, _seed);
            foreach (var feature in _features)
            {
                if (feature == stop)
                    return descriptionContext.CreateDescriptor();
                
                descriptionContext.NeighborDescriptor = p => Describe(p, feature);
                feature.Describe(descriptionContext);
                descriptionContext.Next();
            }
            return descriptionContext.CreateDescriptor();
        }

        public Chunk Generate(ChunkPos pos)
        {
            ChunkDescriptor? descriptor;
            lock (_sliceDescriptors)
            {
                var slicePos = new ChunkPos(pos.X, pos.Z);
                if (!_sliceDescriptors.TryGetValue(slicePos, out descriptor))
                    _sliceDescriptors[slicePos] = descriptor = Describe(slicePos, null);
            }

            var chunk = new Chunk(pos);
            foreach (var feature in _features)
                feature.Populate(descriptor, chunk);
            return chunk;
        }

        public bool TryGet(ChunkPos pos, [NotNullWhen(true)] out Chunk? chunk)
        {
            chunk = Generate(pos);
            return true;
        }
    }
}