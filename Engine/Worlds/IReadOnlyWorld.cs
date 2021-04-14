using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyWorld
    {
        public ulong AbsoluteTime { get; }

        public float Gravity { get; }
        
        public TReadOnly Get<TReadOnly, T>(DataHandle<IWorld, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;

        IReadOnlyChunk? GetChunk(ChunkPos pos, bool load = true);
    }
}