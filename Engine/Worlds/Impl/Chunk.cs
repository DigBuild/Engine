using System;
using DigBuild.Engine.Math;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A basic chunk implementation.
    /// </summary>
    public sealed class Chunk : IChunk
    {
        private readonly DataContainer<IChunk> _data;

        public ChunkPos Position { get; }

        public event Action? Changed;

        public Chunk(ChunkPos position) :
            this(position, new DataContainer<IChunk>())
        {
        }

        private Chunk(ChunkPos position, DataContainer<IChunk> data)
        {
            Position = position;
            _data = data;
            _data.Changed += () => Changed?.Invoke();
        }

        public T Get<TReadOnly, T>(DataHandle<IChunk, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            return _data.Get(handle);
        }

        public static ISerdes<Chunk> Serdes = new SimpleSerdes<Chunk>(
            (stream, chunk) =>
            {
                UnmanagedSerdes<ChunkPos>.NotNull.Serialize(stream, chunk.Position);
                DataContainer<IChunk>.Serdes.Serialize(stream, chunk._data);
            },
            (stream, context) =>
            {
                var pos = UnmanagedSerdes<ChunkPos>.NotNull.Deserialize(stream, context);
                var data = DataContainer<IChunk>.Serdes.Deserialize(stream, context);
                return new Chunk(pos, data);
            }
        );
    }
}