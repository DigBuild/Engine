using System.Collections.Generic;

namespace DigBuild.Engine.Items
{
    public sealed class ItemDataContainer
    {
        private readonly Dictionary<IItemDataHandle, object> _data = new();

        internal T Get<T>(ItemDataHandle<T> handle) where T : class, new()
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            return (T) (_data[handle] = new T());
        }
    }

    public interface IItemDataHandle
    {
    }

    public sealed class ItemDataHandle<T> : IItemDataHandle
    {
    }
}