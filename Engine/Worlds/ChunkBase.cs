using System.Collections.Generic;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public abstract class ChunkBase : IChunk
    {
        private readonly Dictionary<IChunkStorageType, IChunkStorage> _storage = new();

        public abstract ChunkPos Position { get; }

        protected ChunkBase()
        {
            foreach (var type in BuiltInRegistries.ChunkStorageTypes.Values)
                _storage.Add(type, type.Create());
        }

        public T Get<TReadOnly, T>(ChunkStorageType<TReadOnly, T> type)
            where TReadOnly : IReadOnlyChunkStorage
            where T : TReadOnly, IChunkStorage<T>
        {
            return (T) _storage[type];
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