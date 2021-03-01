using System;
using DigBuild.Engine.Util;

namespace DigBuild.Engine.Voxel
{
    public sealed class BlockBuilder
    {
        public LocalHandle<TData> Add<TData>()
            where TData : class, new() => throw new NotImplementedException();
        
        public void Attach(IBlockBehavior<object> behavior) => throw new NotImplementedException();
        public void Attach<TContract, TData>(IBlockBehavior<TContract> behavior, LocalHandle<TData> data)
            where TData : class, TContract, new() => throw new NotImplementedException();
        public void Attach<TContract, TData>(IBlockBehavior<TContract> behavior, LocalHandle<TData> data, Func<TData, TContract> adapter)
            where TData : class, new() => throw new NotImplementedException();
    }
}