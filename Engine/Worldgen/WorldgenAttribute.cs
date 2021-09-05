using DigBuild.Engine.Collections;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worldgen
{
    /// <summary>
    /// A world generation attribute.
    /// </summary>
    public interface IWorldgenAttribute { }

    /// <summary>
    /// A world generation attribute.
    /// </summary>
    /// <typeparam name="T">The attribute type</typeparam>
    public sealed class WorldgenAttribute<T> : IWorldgenAttribute
        where T : notnull
    {
        internal WorldgenAttribute()
        {
        }
    }

    /// <summary>
    /// Registry extensions for world generation attributes.
    /// </summary>
    public static class WorldgenAttributeRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new worldgen attribute of the specified type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The attribute name</param>
        /// <returns>The attribute</returns>
        public static WorldgenAttribute<T> Register<T>(this RegistryBuilder<IWorldgenAttribute> registry, ResourceName name)
            where T : notnull
        {
            return ((IRegistryBuilder<IWorldgenAttribute>)registry).Add(name, new WorldgenAttribute<T>());
        }
    }

    /// <summary>
    /// Chunk description context extensions for world generation attributes.
    /// </summary>
    public static class WorldgenAttributeDescriptionContextExtensions
    {
        /// <summary>
        /// Gets a grid type attribute as an extended grid.
        /// </summary>
        /// <typeparam name="T">The grid type</typeparam>
        /// <param name="context">The context</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>The extended grid</returns>
        public static ExtendedWorldgenGrid<T> GetExtendedGrid<T>(this ChunkDescriptionContext context, WorldgenAttribute<Grid<T>> attribute)
        {
            return new ExtendedWorldgenGrid<T>(context, attribute);
        }
    }
}