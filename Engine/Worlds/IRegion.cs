using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyRegion
    {
        RegionPos Position { get; }

        bool IsLoaded(RegionChunkPos pos);

        IReadOnlyChunk? Get(RegionChunkPos pos, bool loadOrGenerate = true);

        bool TryGet(RegionChunkPos pos, [NotNullWhen(true)] out IReadOnlyChunk? chunk, bool loadOrGenerate = true)
        {
            return (chunk = Get(pos, loadOrGenerate)) != null;
        }
        
        TReadOnly Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
    }

    public interface IRegion : IReadOnlyRegion
    {
        public const uint Size = WorldDimensions.RegionSize;

        new IChunk? Get(RegionChunkPos pos, bool loadOrGenerate = true);

        bool TryGet(RegionChunkPos pos, [NotNullWhen(true)] out IChunk? chunk, bool loadOrGenerate = true)
        {
            return (chunk = Get(pos, loadOrGenerate)) != null;
        }
        
        new T Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        
        IReadOnlyChunk? IReadOnlyRegion.Get(RegionChunkPos pos, bool loadOrGenerate) => Get(pos, loadOrGenerate);
        bool IReadOnlyRegion.TryGet(RegionChunkPos pos, [NotNullWhen(true)] out IReadOnlyChunk? chunk, bool loadOrGenerate)
        {
            return (chunk = Get(pos, loadOrGenerate)) != null;
        }
        TReadOnly IReadOnlyRegion.Get<TReadOnly, T>(DataHandle<IRegion, TReadOnly, T> type) => Get(type);
    }
}