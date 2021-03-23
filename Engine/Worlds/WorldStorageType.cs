using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worlds
{
    public interface IWorldStorageType
    {
        internal IWorldStorage Create();
    }

    public sealed class WorldStorageType<TReadOnly, T> : IWorldStorageType
        where TReadOnly : IReadOnlyWorldStorage
        where T : TReadOnly, IWorldStorage<T>
    {
        private readonly Func<T> _factory;

        internal WorldStorageType(Func<T> factory)
        {
            _factory = factory;
        }
        
        IWorldStorage IWorldStorageType.Create() => _factory();
    }

    public static class WorldStorageTypeRegistryBuilderExtensions
    {
        public static WorldStorageType<TReadOnly, T> Create<TReadOnly, T>(this IRegistryBuilder<IWorldStorageType> registry, ResourceName name)
            where TReadOnly : IReadOnlyWorldStorage
            where T : class, TReadOnly, IWorldStorage<T>, new()
        {
            return registry.Add(name, new WorldStorageType<TReadOnly, T>(() => new T()));
        }
    }
}