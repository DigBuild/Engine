using System;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public sealed class Chunk : IChunk
    {
        private readonly DataContainer<IChunk> _data = new();

        public ChunkPos Position { get; }

        public event Action? Changed;

        public Chunk(ChunkPos position)
        {
            Position = position;
            _data.Changed += () => Changed?.Invoke();
        }

        public T Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            return _data.Get(type);
        }
    }
}