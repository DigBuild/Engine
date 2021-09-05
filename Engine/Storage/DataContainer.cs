using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    internal sealed class DataContainer : IChangeNotifier
    {
        private readonly Dictionary<IDataHandle, IData> _data;
        private readonly LockStore<IDataHandle> _locks = new();

        internal IReadOnlyDictionary<IDataHandle, IData> Entries => _data;

        public event Action? Changed;
        
        public DataContainer()
        {
            _data = new Dictionary<IDataHandle, IData>();
        }
        
        public DataContainer(IReadOnlyDictionary<IDataHandle, IData> entries, bool copy)
        {
            if (copy)
            {
                _data = new Dictionary<IDataHandle, IData>();
                foreach (var (handle, data) in entries)
                    _data[handle] = data.Copy();
            }
            else
            {
                _data = new Dictionary<IDataHandle, IData>(entries);
            }

            foreach (var data in entries.Values)
                if (data is IChangeNotifier notifier)
                    notifier.Changed += NotifyChange;
        }

        private void NotifyChange() => Changed?.Invoke();

        public T Get<T>(DataHandle<T> handle) where T : class, IData<T>
        {
            using var lck = _locks.Lock(handle);

            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            var t = handle.New();
            _data[handle] = t;
            if (t is IChangeNotifier notifier)
                notifier.Changed += NotifyChange;
            return t;
        }

        public DataContainer Copy()
        {
            return new DataContainer(_data, true);
        }
    }

    /// <summary>
    /// A data container with a specific target type. The registry property must be populated during initialization.
    /// </summary>
    /// <typeparam name="TTarget">The target type</typeparam>
    public sealed class DataContainer<TTarget> : IChangeNotifier
    {
        /// <summary>
        /// The registry the data handles are added to. Must be populated during initialization.
        /// </summary>
        public static Registry<IDataHandle<TTarget>> Registry { private get; set; } = null!;
        
        private readonly Dictionary<IDataHandle<TTarget>, IData> _data = new();
        private readonly LockStore<IDataHandle<TTarget>> _locks = new();

        public event Action? Changed;

        private void NotifyChange()
        {
            Changed?.Invoke();
        }

        /// <summary>
        /// Gets the value for a specific target, or creates a new one if missing.
        /// </summary>
        /// <typeparam name="TReadOnly">The read-only type</typeparam>
        /// <typeparam name="T">The read-write type</typeparam>
        /// <param name="handle">The handle</param>
        /// <returns>The value</returns>
        public T Get<TReadOnly, T>(DataHandle<TTarget, TReadOnly, T> handle)
            where T : TReadOnly, IData<T>
        {
            using var lck = _locks.Lock(handle);

            if (_data.TryGetValue(handle, out var data))
                return (T) data;
            var t = handle.New();
            if (t is IChangeNotifier notifier)
                notifier.Changed += NotifyChange;
            _data[handle] = t;
            return t;
        }

        /// <summary>
        /// Creates a new deep copy of this data container.
        /// </summary>
        /// <returns>The copy</returns>
        public DataContainer<TTarget> Copy()
        {
            var copy = new DataContainer<TTarget>();
            foreach (var (handle, data) in _data)
            {
                var t = copy._data[handle] = data.Copy();
                if (t is IChangeNotifier notifier)
                    notifier.Changed += copy.NotifyChange;
            }

            return copy;
        }

        /// <summary>
        /// The serdes.
        /// </summary>
        public static ISerdes<DataContainer<TTarget>> Serdes { get; } = new SimpleSerdes<DataContainer<TTarget>>(
            (stream, container) =>
            {
                var bw = new BinaryWriter(stream);
                bw.Write(container._data.Keys.Count(k => k.Name != null));

                foreach (var (key, value) in container._data)
                {
                    var name = key.Name;
                    if (name == null)
                        continue;
                    bw.Write(name.Value.ToString());
                    key.Serialize(stream, value);
                }
            },
            (stream, context) =>
            {
                var br = new BinaryReader(stream);
                var count = br.ReadInt32();

                var container = new DataContainer<TTarget>();

                for (var i = 0; i < count; i++)
                {
                    var name = ResourceName.Parse(br.ReadString())!;
                    var handle = Registry.GetOrNull(name.Value)!;
                    var data = handle.Deserialize(stream, context);
                    container._data[handle] = data;
                    ((IChangeNotifier) data).Changed += container.NotifyChange;
                }

                return container;
            }
        );
    }
}