using System;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public sealed class LowDensityRegion : ILowDensityRegion
    {
        private readonly DataContainer<ILowDensityRegion> _data = new();

        public event Action? Changed;

        public LowDensityRegion()
        {
            _data.Changed += () => Changed?.Invoke();
        }

        public T Get<TReadOnly, T>(DataHandle<ILowDensityRegion, TReadOnly, T> type)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            return _data.Get(type);
        }
    }
}