using System;
using System.Collections.Generic;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;

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

        public abstract ulong AbsoluteTime { get; }

        public abstract IChunkManager ChunkManager { get; }

        public abstract Scheduler TickScheduler { get; }

        public abstract IChunk? GetChunk(ChunkPos pos, bool load = true);

        public abstract void OnBlockChanged(BlockPos pos);

        public abstract void OnEntityAdded(EntityInstance entity);

        public abstract void OnEntityRemoved(Guid guid);
    }
}