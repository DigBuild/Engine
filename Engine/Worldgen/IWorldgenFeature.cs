using System.Collections.Immutable;
using DigBuildEngine.Reg;
using DigBuildEngine.Util;
using DigBuildPlatformCS.Resource;

namespace DigBuildEngine.Worldgen
{
    public interface IWorldgenFeature
    {
        IImmutableSet<IWorldgenAttribute> InputAttributes { get; }
        IImmutableSet<IWorldgenAttribute> OutputAttributes { get; }

        void DescribeSlice(WorldSliceDescriptionContext context);
        
        void PopulateChunk(WorldSliceDescriptor descriptor, ChunkPrototype chunk);
    }

    public static class WorldgenFeatureRegistryBuilderExtensions
    {
        public static T Add<T>(this RegistryBuilder<IWorldgenFeature> registry, ResourceName name, T feature) where T : IWorldgenFeature
        {
            ((IRegistryBuilder<IWorldgenFeature>)registry).Add(name, feature);
            return feature;
        }
    }
}