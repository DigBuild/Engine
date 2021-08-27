using DigBuild.Engine.Entities;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Worlds
{
    public interface IWorld : IReadOnlyWorld
    {
        IChunkManager ChunkManager { get; }

        Scheduler TickScheduler { get; }

        new T Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        TReadOnly IReadOnlyWorld.Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> type) => Get(type);

        new IChunk? GetChunk(ChunkPos pos, bool loadOrGenerate = true);

        IReadOnlyChunk? IReadOnlyWorld.GetChunk(ChunkPos pos, bool loadOrGenerate) => GetChunk(pos, loadOrGenerate);

        void OnBlockChanged(BlockPos pos);
        void OnEntityAdded(EntityInstance entity);
        void OnEntityRemoving(EntityInstance entity);

        void MarkChunkForReRender(ChunkPos pos);
        void MarkBlockForReRender(BlockPos pos);
    }
}