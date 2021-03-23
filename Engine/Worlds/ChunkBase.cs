using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public abstract class ChunkBase : IChunk
    {
        private readonly Dictionary<Type, IChunkStorage> _storage = new();

        public abstract ChunkPos Position { get; }

        public T Get<T>() where T : class, IChunkStorage<T>, new()
        {
            if (!_storage.TryGetValue(typeof(T), out var storage))
                _storage[typeof(T)] = storage = new T();
            return (T) storage;
        }

        public void CopyFrom(IReadOnlyChunk other)
        {
            if (other is not ChunkBase other2)
                return;

            _storage.Clear();
            foreach (var (type, storage) in other2._storage)
                _storage[type] = storage.Copy();
        }
    }
}