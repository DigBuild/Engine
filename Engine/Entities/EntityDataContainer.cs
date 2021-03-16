using System.Collections.Generic;

namespace DigBuild.Engine.Entities
{
    public sealed class EntityDataContainer
    {
        private readonly Dictionary<IEntityDataHandle, object> _data = new();

        internal T Get<T>(EntityDataHandle<T> handle) where T : class, new()
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            return (T) (_data[handle] = new T());
        }
    }

    public interface IEntityDataHandle
    {
    }

    public sealed class EntityDataHandle<T> : IEntityDataHandle
    {
    }
}