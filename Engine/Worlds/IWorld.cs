using System;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds
{
    public interface IWorld : IReadOnlyWorld
    {
        // public new T Get<T>() where T : IWorldStorage;

        public IChunkManager ChunkManager { get; }

        public Scheduler TickScheduler { get; }

        public new IChunk? GetChunk(ChunkPos pos, bool load = true);

        IReadOnlyChunk? IReadOnlyWorld.GetChunk(ChunkPos pos, bool load) => GetChunk(pos, load);

        public void OnBlockChanged(BlockPos pos);

        public void OnEntityAdded(EntityInstance entity);

        public void OnEntityRemoved(Guid guid);
    }

    public interface IWorldStorage : IReadOnlyWorldStorage
    {
    }
    public interface IWorldStorage<out T> : IWorldStorage where T : class, IWorldStorage<T>, new()
    {
    }
}