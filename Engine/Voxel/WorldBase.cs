using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public abstract class WorldBase : IWorld
    {
        private readonly Dictionary<Type, IWorldStorage> _storage = new();

        public T Get<T>() where T : class, IWorldStorage<T>, new()
        {
            if (!_storage.TryGetValue(typeof(T), out var storage))
                _storage[typeof(T)] = storage = new T();
            return (T) storage;
        }

        public abstract IChunk? GetChunk(ChunkPos pos, bool load = true);

        public abstract void OnBlockChanged(BlockPos pos);
    }
}