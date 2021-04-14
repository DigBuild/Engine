using System;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public abstract class WorldBase : IWorld, IDisposable
    {
        private readonly DataContainer<IWorld> _storage = new();

        public abstract ulong AbsoluteTime { get; }

        public abstract float Gravity { get; }

        public RegionManager RegionManager { get; }

        public IChunkManager ChunkManager => RegionManager;

        public abstract Scheduler TickScheduler { get; }

        protected WorldBase(IChunkProvider chunkProvider, Func<RegionPos, IRegionStorage> storageProvider, ITickSource tickSource)
        {
            RegionManager = new RegionManager(chunkProvider, storageProvider, tickSource);
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

        public abstract IChunk? GetChunk(ChunkPos pos, bool loadOrGenerate = true);

        public abstract void OnBlockChanged(BlockPos pos);

        public abstract void OnEntityAdded(EntityInstance entity);

        public abstract void OnEntityRemoved(Guid guid);
    }
}