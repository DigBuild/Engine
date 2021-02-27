using DigBuildEngine.Reg;
using DigBuildEngine.Util;
using DigBuildPlatformCS.Resource;

namespace DigBuildEngine.Worldgen
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
}