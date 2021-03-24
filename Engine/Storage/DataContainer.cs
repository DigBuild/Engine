using System.Collections.Generic;

namespace DigBuild.Engine.Storage
{
    internal sealed class DataContainer
    {
        private readonly Dictionary<IDataHandle, object> _data = new();

        internal T Get<T>(DataHandle<T> handle) where T : class, new()
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            return (T) (_data[handle] = new T());
        }
    }
}