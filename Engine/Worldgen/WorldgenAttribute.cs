using DigBuild.Engine.Collections;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worldgen
{
    public interface IWorldgenAttribute { }
    public sealed class WorldgenAttribute<TStorage> : IWorldgenAttribute
        where TStorage : notnull
    {
        internal WorldgenAttribute()
        {
        }
    }

    public static class WorldgenAttributeRegistryBuilderExtensions
    {
        public static WorldgenAttribute<TStorage> Create<TStorage>(this RegistryBuilder<IWorldgenAttribute> registry, ResourceName name)
            where TStorage : notnull
        {
            var attribute = new WorldgenAttribute<TStorage>();
            ((IRegistryBuilder<IWorldgenAttribute>)registry).Add(name, attribute);
            return attribute;
        }
    }

    public static class WorldgenAttributeSliceExtensions
    {
        public static ExtendedGrid<T> GetExtendedGrid<T>(this ChunkDescriptionContext context, WorldgenAttribute<Grid<T>> attribute)
        {
            return new(context, attribute);
        }
    }
}