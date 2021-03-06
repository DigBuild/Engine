using System;
using System.Collections.Generic;

namespace DigBuild.Engine.Blocks
{
    public sealed class BlockDataContainer
    {
        private readonly Dictionary<IBlockDataHandle, object> _data = new();

        internal T Get<T>(BlockDataHandle<T> handle) where T : class, new()
        {
            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            return (T) (_data[handle] = new T());
        }
    }

    public interface IBlockDataHandle
    {
    }

    public sealed class BlockDataHandle<T> : IBlockDataHandle
    {
    }
}