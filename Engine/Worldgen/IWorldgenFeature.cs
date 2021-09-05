using DigBuild.Engine.Registries;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worldgen
{
    /// <summary>
    /// A world generation feature.
    /// </summary>
    public interface IWorldgenFeature
    {
        /// <summary>
        /// Populates this feature's attribute descriptors for the current chunk, optionally querying the surrounding area.
        /// </summary>
        /// <param name="context">The description context</param>
        void Describe(ChunkDescriptionContext context);
        
        /// <summary>
        /// Populates the chunk based on the previously created attribute descriptors.
        /// </summary>
        /// <param name="descriptor">The chunk descriptor</param>
        /// <param name="chunk">The chunk</param>
        void Populate(ChunkDescriptor descriptor, IChunk chunk);
    }

    /// <summary>
    /// Registry extensions for world generation features.
    /// </summary>
    public static class WorldgenFeatureRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new world generation feature.
        /// </summary>
        /// <typeparam name="T">The feature type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The feature name</param>
        /// <param name="feature">The feature</param>
        /// <returns>The feature</returns>
        public static T Register<T>(this RegistryBuilder<IWorldgenFeature> registry, ResourceName name, T feature) where T : IWorldgenFeature
        {
            return ((IRegistryBuilder<IWorldgenFeature>)registry).Add(name, feature);
        }
    }
}