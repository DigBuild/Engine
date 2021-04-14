using System;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds
{
    public interface IWorld : IReadOnlyWorld
    {
        public IChunkManager ChunkManager { get; }

        public Scheduler TickScheduler { get; }
        public new T Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        TReadOnly IReadOnlyWorld.Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> type) => Get(type);

        public new IChunk? GetChunk(ChunkPos pos, bool loadOrGenerate = true);

        IReadOnlyChunk? IReadOnlyWorld.GetChunk(ChunkPos pos, bool loadOrGenerate) => GetChunk(pos, loadOrGenerate);

        public void OnBlockChanged(BlockPos pos);

        public void OnEntityAdded(EntityInstance entity);

        public void OnEntityRemoved(Guid guid);
    }
}