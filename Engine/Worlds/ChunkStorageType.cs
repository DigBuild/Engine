using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worlds
{
    public interface IChunkStorageType
    {
        internal IChunkStorage Create();
    }

    public sealed class ChunkStorageType<TReadOnly, T> : IChunkStorageType
        where TReadOnly : IReadOnlyChunkStorage
        where T : TReadOnly, IChunkStorage<T>
    {
        private readonly Func<T> _factory;

        internal ChunkStorageType(Func<T> factory)
        {
            _factory = factory;
        }
        
        IChunkStorage IChunkStorageType.Create() => _factory();
    }

    public static class ChunkStorageTypeRegistryBuilderExtensions
    {
        public static ChunkStorageType<TReadOnly, T> Create<TReadOnly, T>(this IRegistryBuilder<IChunkStorageType> registry, ResourceName name)
            where TReadOnly : IReadOnlyChunkStorage
            where T : class, TReadOnly, IChunkStorage<T>, new()
        {
            return registry.Add(name, new ChunkStorageType<TReadOnly, T>(() => new T()));
        }
    }
}