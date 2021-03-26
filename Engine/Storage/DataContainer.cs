using System.Collections.Generic;

namespace DigBuild.Engine.Storage
{
    internal sealed class DataContainer
    {
        private readonly Dictionary<IDataHandle, IData> _data = new();

        internal T Get<T>(DataHandle<T> handle) where T : class, IData<T>, new()
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            return (T) (_data[handle] = new T());
        }

        public DataContainer Copy()
        {
            var copy = new DataContainer();
            foreach (var (handle, data) in _data)
                copy._data[handle] = data.Copy();
            return copy;
        }
    }
}