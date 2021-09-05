using System;
using System.IO;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    /// <summary>
    /// An opaque handle for a given data type.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
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

    /// <summary>
    /// An opaque handle for a specific data type on a target.
    /// </summary>
    /// <typeparam name="TTarget">The target</typeparam>
    /// <typeparam name="TReadOnly">The read-only type</typeparam>
    /// <typeparam name="T">The read-write type</typeparam>
    public sealed class DataHandle<TTarget, TReadOnly, T> : IDataHandle<TTarget> where T : TReadOnly, IData<T>
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
 
    /// <summary>
    /// Registry extensions for data handles.
    /// </summary>
    public static class DataHandleRegistryExtensions
    {
        /// <summary>
        /// Registers a new data handle with the specified name and serdes.
        /// </summary>
        /// <typeparam name="TTarget">The target</typeparam>
        /// <typeparam name="TReadOnly">The read-only type</typeparam>
        /// <typeparam name="T">The read-write type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="serdes">The serdes</param>
        /// <returns>The data handle</returns>
        public static DataHandle<TTarget, TReadOnly, T> Register<TTarget, TReadOnly, T>(
            this IRegistryBuilder<IDataHandle<TTarget>> registry,
            ResourceName name,
            ISerdes<T> serdes
        )
            where T : class, TReadOnly, IData<T>, IChangeNotifier, new() 
        { 
            return registry.Add(name, new DataHandle<TTarget, TReadOnly, T>(() => new T(), name, serdes));
        }
 
        /// <summary>
        /// Registers a new data handle with the specified name and serdes, as well as a custom instance factory.
        /// </summary>
        /// <typeparam name="TTarget">The target</typeparam>
        /// <typeparam name="TReadOnly">The read-only type</typeparam>
        /// <typeparam name="T">The read-write type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="factory"></param>
        /// <param name="serdes">The serdes</param>
        /// <returns>The data handle</returns>
        public static DataHandle<TTarget, TReadOnly, T> Register<TTarget, TReadOnly, T>(
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