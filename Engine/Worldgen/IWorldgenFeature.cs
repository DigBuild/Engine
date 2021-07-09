using System.Collections.Immutable;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worldgen
{
    public interface IWorldgenFeature
    {
        IImmutableSet<IWorldgenAttribute> InputAttributes { get; }
        IImmutableSet<IWorldgenAttribute> OutputAttributes { get; }

        void Describe(ChunkDescriptionContext context);
        
        void Populate(ChunkDescriptor descriptor, IChunk chunk);
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