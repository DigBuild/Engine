using System;
using System.IO;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    public interface IDataHandle
    {
        internal ResourceName? Name { get; }

        internal void Serialize(Stream stream, IData obj);

        internal IData Deserialize(Stream stream, IDeserializationContext context);
    }
    public interface IDataHandle<TTarget> : IDataHandle
    {
    }

    public sealed class DataHandle<T> : IDataHandle where T : class, IData<T>
    {
        private readonly Func<T> _factory;
        private readonly ResourceName? _name;
        private readonly ISerdes<T>? _serdes;
        
        ResourceName? IDataHandle.Name => _name;

        internal DataHandle(Func<T> factory)
        {
            _factory = factory;
        }

        internal DataHandle(Func<T> factory, ResourceName name, ISerdes<T> serdes)
        {
            _factory = factory;
            _name = name;
            _serdes = serdes;
        }

        internal T New() => _factory();

        void IDataHandle.Serialize(Stream stream, IData obj)
        {
            _serdes!.Serialize(stream, (T) obj);
        }

        IData IDataHandle.Deserialize(Stream stream, IDeserializationContext context)
        {
            return _serdes!.Deserialize(stream, context);
        }
    }

    public sealed class DataHandle<TTarget, TReadOnly, T> : IDataHandle<TTarget> where T : TReadOnly, IData<T>, IChangeNotifier
    {
        private readonly Func<T> _factory;
        private readonly ResourceName _name;
        private readonly ISerdes<T>? _serdes;
        
        ResourceName? IDataHandle.Name => _serdes == null ? null : _name;

        internal DataHandle(Func<T> factory, ResourceName name, ISerdes<T>? serdes)
        {
            _factory = factory;
            _name = name;
            _serdes = serdes;
        }

        internal T New() => _factory();

        void IDataHandle.Serialize(Stream stream, IData obj)
        {
            _serdes!.Serialize(stream, (T) obj);
        }

        IData IDataHandle.Deserialize(Stream stream, IDeserializationContext context)
        {
            return _serdes!.Deserialize(stream, context);
        }
    }
 
    public static class DataHandlerExtensions
    {
        public static DataHandle<TTarget, TReadOnly, T> Create<TTarget, TReadOnly, T>(
            this IRegistryBuilder<IDataHandle<TTarget>> registry,
            ResourceName name,
            ISerdes<T> serdes
        )
            where T : class, TReadOnly, IData<T>, IChangeNotifier, new() 
        { 
            return registry.Add(name, new DataHandle<TTarget, TReadOnly, T>(() => new T(), name, serdes));
        }
 
        public static DataHandle<TTarget, TReadOnly, T> Create<TTarget, TReadOnly, T>(
            this IRegistryBuilder<IDataHandle<TTarget>> registry,
            ResourceName name,
            Func<T> factory,
            ISerdes<T> serdes
        )
            where T : class, TReadOnly, IData<T>, IChangeNotifier
        {
            return registry.Add(name, new DataHandle<TTarget, TReadOnly, T>(factory, name, serdes));
        }
    }
}