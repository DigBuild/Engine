using System;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Events;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds.Impl
{
    public abstract class WorldBase : IWorld, IDisposable
    {
        private readonly DataContainer<IWorld> _storage = new();

        public abstract ulong AbsoluteTime { get; }

        public abstract float Gravity { get; }

        public RegionManager RegionManager { get; }

        public IChunkManager ChunkManager => RegionManager;

        public abstract Scheduler TickScheduler { get; }

        protected WorldBase(
            ITickSource tickSource,
            IChunkProvider chunkProvider,
            Func<RegionPos, IRegionStorage> storageProvider,
            EventBus eventBus
        )
        {
            RegionManager = new RegionManager(this, chunkProvider, storageProvider, tickSource, eventBus);
        }

        public virtual void Dispose()
        {
            RegionManager.Dispose();
        }

        public T Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            return _storage.Get(type);
        }

        public virtual IChunk? GetChunk(ChunkPos pos, bool loadOrGenerate = true)
        {
            return RegionManager.Get(pos.RegionPos)?.Get(pos.RegionChunkPos, loadOrGenerate);
        }

        public abstract void OnBlockChanged(BlockPos pos);
        public abstract void OnEntityAdded(EntityInstance entity);
        public abstract void OnEntityRemoving(EntityInstance entity);

        public abstract void MarkChunkForReRender(ChunkPos pos);
        public abstract void MarkBlockForReRender(BlockPos pos);
    }
}