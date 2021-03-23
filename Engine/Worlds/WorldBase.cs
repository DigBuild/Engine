using System;
using System.Collections.Generic;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds
{
    public abstract class WorldBase : IWorld
    {
        private readonly Dictionary<IWorldStorageType, IWorldStorage> _storage = new();

        public abstract ulong AbsoluteTime { get; }

        public abstract IChunkManager ChunkManager { get; }

        public abstract Scheduler TickScheduler { get; }

        protected WorldBase()
        {
            foreach (var type in BuiltInRegistries.WorldStorageTypes.Values)
                _storage.Add(type, type.Create());
        }

        public T Get<TReadOnly, T>(WorldStorageType<TReadOnly, T> type)
            where TReadOnly : IReadOnlyWorldStorage
            where T : TReadOnly, IWorldStorage<T>
        {
            return (T) _storage[type];
        }

        public abstract IChunk? GetChunk(ChunkPos pos, bool load = true);

        public abstract void OnBlockChanged(BlockPos pos);

        public abstract void OnEntityAdded(EntityInstance entity);

        public abstract void OnEntityRemoved(Guid guid);
    }
}