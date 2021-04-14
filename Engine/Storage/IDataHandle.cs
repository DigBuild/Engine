using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    public interface IDataHandle
    {
    }
    public interface IDataHandle<TTarget> : IDataHandle
    {
    }

    public sealed class DataHandle<T> : IDataHandle where T : class, IData<T>
    {
        private readonly Func<T> _factory;

        internal DataHandle(Func<T> factory)
        {
            _factory = factory;
        }

        internal T New() => _factory();
    }

    public sealed class DataHandle<TTarget, TReadOnly, T> : IDataHandle<TTarget> where T : TReadOnly, IData<T>, IChangeNotifier
    {
        private readonly Func<T> _factory;

        internal DataHandle(Func<T> factory)
        {
            _factory = factory;
        }

        internal T New() => _factory();
    }
 
    public static class DataHandlerExtensions
    {
        public static DataHandle<TTarget, TReadOnly, T> Create<TTarget, TReadOnly, T>(
            this IRegistryBuilder<IDataHandle<TTarget>> registry,
            ResourceName name
        )
            where T : class, TReadOnly, IData<T>, IChangeNotifier, new() 
        { 
            return registry.Add(name, new DataHandle<TTarget, TReadOnly, T>(() => new T()));
        }
 
        public static DataHandle<TTarget, TReadOnly, T> Create<TTarget, TReadOnly, T>(
            this IRegistryBuilder<IDataHandle<TTarget>> registry,
            ResourceName name,
            Func<T> factory
        )
            where T : class, TReadOnly, IData<T>, IChangeNotifier
        {
            return registry.Add(name, new DataHandle<TTarget, TReadOnly, T>(factory));
        }
    }
}