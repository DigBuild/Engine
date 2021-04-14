using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyLowDensityRegion
    {
        TReadOnly Get<TReadOnly, T>(DataHandle<ILowDensityRegion, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
    }

    public interface ILowDensityRegion : IReadOnlyLowDensityRegion, IChangeNotifier
    {
        new T Get<TReadOnly, T>(DataHandle<ILowDensityRegion, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier;
        
        TReadOnly IReadOnlyLowDensityRegion.Get<TReadOnly, T>(DataHandle<ILowDensityRegion, TReadOnly, T> type) => Get(type);
    }
}