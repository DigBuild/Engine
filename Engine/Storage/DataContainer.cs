using System;
using System.Collections.Generic;
using DigBuild.Engine.Serialization;

namespace DigBuild.Engine.Storage
{
    internal sealed class DataContainer
    {
        private readonly Dictionary<IDataHandle, IData> _data = new();

        public T Get<T>(DataHandle<T> handle) where T : class, IData<T>
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            var t = handle.New();
            _data[handle] = t;
            return t;
        }

        public DataContainer Copy()
        {
            var copy = new DataContainer();
            foreach (var (handle, data) in _data)
                copy._data[handle] = data.Copy();
            return copy;
        }

        public static ISerdes<DataContainer> Serdes { get; } = new SimpleSerdes<DataContainer>(
            (stream, container) => { },
            stream => new DataContainer()
        );
    }

    public sealed class DataContainer<TTarget> : IChangeNotifier
    {
        private readonly Dictionary<IDataHandle, IData> _data = new();

        public event Action? Changed;

        private void OnChange()
        {
            Changed?.Invoke();
        }

        public T Get<TReadOnly, T>(DataHandle<TTarget, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>, IChangeNotifier
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            var t = handle.New();
            t.Changed += OnChange;
            _data[handle] = t;
            return t;
        }

        public DataContainer<TTarget> Copy()
        {
            var copy = new DataContainer<TTarget>();
            foreach (var (handle, data) in _data)
                copy._data[handle] = data.Copy();
            return copy;
        }

        public static ISerdes<DataContainer<TTarget>> Serdes { get; } = new SimpleSerdes<DataContainer<TTarget>>(
            (stream, container) => { },
            stream => new DataContainer<TTarget>()
        );
    }
}