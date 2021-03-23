using System;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds
{
    public interface IWorld : IReadOnlyWorld
    {
        public IChunkManager ChunkManager { get; }

        public Scheduler TickScheduler { get; }
        public new T Get<TReadOnly, T>(WorldStorageType<TReadOnly, T> type)
            where TReadOnly : IReadOnlyWorldStorage
            where T : TReadOnly, IWorldStorage<T>;
        TReadOnly IReadOnlyWorld.Get<TReadOnly, T>(WorldStorageType<TReadOnly, T> type) => Get(type);

        public new IChunk? GetChunk(ChunkPos pos, bool load = true);

        IReadOnlyChunk? IReadOnlyWorld.GetChunk(ChunkPos pos, bool load) => GetChunk(pos, load);

        public void OnBlockChanged(BlockPos pos);

        public void OnEntityAdded(EntityInstance entity);

        public void OnEntityRemoved(Guid guid);
    }
}