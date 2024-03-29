﻿using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public delegate ref TOut RefFunc<in TIn, TOut>(TIn input);
    
    internal delegate object GenericBlockEventDelegate(IBlockEvent evt, DataContainer dataContainer);
    internal delegate object GenericBlockAttributeDelegate(IReadOnlyBlockContext context, DataContainer dataContainer);
    internal delegate object GenericBlockCapabilityDelegate(IBlockContext context, DataContainer dataContainer);

    /// <summary>
    /// A block builder.
    /// </summary>
    public sealed class BlockBuilder
    {
        private static readonly Func<DataContainer?> CreateNullData = () => null;
        
        private readonly List<IDataHandle> _dataHandles = new();
        private readonly Dictionary<IDataHandle, (ResourceName Name, ISerdes<IData> Serdes)> _serializedDataHandles = new();
        private readonly Dictionary<Type, List<BlockEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> _capabilitySuppliers = new();
        private readonly List<Action<DataContainer>> _dataInitializers = new();

        /// <summary>
        /// Adds a new non-persistent data class.
        /// </summary>
        /// <typeparam name="TData">The data class type</typeparam>
        /// <returns>The handle</returns>
        public DataHandle<TData> Add<TData>()
            where TData : class, IData<TData>, new()
        {
            var handle = new DataHandle<TData>(() => new TData());
            _dataHandles.Add(handle);
            return handle;
        }

        /// <summary>
        /// Adds a persistent data class.
        /// </summary>
        /// <typeparam name="TData">The data class type</typeparam>
        /// <param name="name">The name</param>
        /// <param name="serdes">The serdes</param>
        /// <returns>The handle</returns>
        public DataHandle<TData> Add<TData>(ResourceName name, ISerdes<TData> serdes)
            where TData : class, IData<TData>, IChangeNotifier, new()
        {
            var handle = Add<TData>();
            _serializedDataHandles.Add(handle, (name, serdes.UncheckedSuperCast<TData, IData>()));
            return handle;
        }

        /// <summary>
        /// Attaches a behavior without a contract.
        /// </summary>
        /// <param name="behavior">The behavior</param>
        public void Attach(IBlockBehavior behavior) => AttachLast(behavior);
        /// <summary>
        /// Attaches a behavior.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        public void Attach<TReadOnlyContract, TContract, TData>(IBlockBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
            => AttachLast(behavior, data);
        /// <summary>
        /// Attaches a behavior with deferred data querying.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        /// <param name="adapter">The data adapter</param>
        public void Attach<TReadOnlyContract, TContract, TData>(IBlockBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
            => AttachLast(behavior, data, adapter);
        
        /// <summary>
        /// Attaches a behavior without a contract at the end of the chain.
        /// </summary>
        /// <param name="behavior">The behavior</param>
        public void AttachLast(IBlockBehavior behavior)
        {
            var builder = new BlockBehaviorBuilder<object, object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, false);
        }
        /// <summary>
        /// Attaches a behavior at the end of the chain.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        public void AttachLast<TReadOnlyContract, TContract, TData>(IBlockBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TReadOnlyContract, TContract>(container => container?.Get(data)!);
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(container.Get(data)));
        }
        /// <summary>
        /// Attaches a behavior with deferred data querying at the end of the chain.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        /// <param name="adapter">The data adapter</param>
        public void AttachLast<TReadOnlyContract, TContract, TData>(IBlockBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container?.Get(data)!));
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(adapter(container.Get(data))));
        }
        
        /// <summary>
        /// Attaches a behavior without a contract at the start of the chain.
        /// </summary>
        /// <param name="behavior">The behavior</param>
        public void AttachFirst(IBlockBehavior behavior)
        {
            var builder = new BlockBehaviorBuilder<object, object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, true);
        }
        /// <summary>
        /// Attaches a behavior at the start of the chain.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IBlockBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TReadOnlyContract, TContract>(container => container?.Get(data)!);
            behavior.Build(builder);
            Attach(builder, true);
            
            _dataInitializers.Insert(0, container => behavior.Init(container.Get(data)));
        }
        /// <summary>
        /// Attaches a behavior with deferred data querying at the start of the chain.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        /// <param name="adapter">The data adapter</param>
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IBlockBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container?.Get(data)!));
            behavior.Build(builder);
            Attach(builder, true);
            
            _dataInitializers.Insert(0, container => behavior.Init(adapter(container.Get(data))));
        }

        private void Attach(IBlockBehaviorBuilder builder, bool prepend)
        {
            foreach (var (type, list) in builder.EventHandlers)
            {
                if (!_eventHandlers.TryGetValue(type, out var blockList))
                    _eventHandlers[type] = blockList = new List<BlockEventDelegate>();
                blockList.InsertRange(prepend ? 0 : blockList.Count, list);
            }
            foreach (var (attribute, list) in builder.AttributeSuppliers)
            {
                if (!_attributeSuppliers.TryGetValue(attribute, out var blockList))
                    _attributeSuppliers[attribute] = blockList = new List<BlockAttributeDelegate>();
                blockList.InsertRange(prepend ? 0 : blockList.Count, list);
            }
            foreach (var (capability, list) in builder.CapabilitySuppliers)
            {
                if (!_capabilitySuppliers.TryGetValue(capability, out var blockList))
                    _capabilitySuppliers[capability] = blockList = new List<BlockCapabilityDelegate>();
                blockList.InsertRange(prepend ? 0 : blockList.Count, list);
            }
        }

        internal Block Build(
            ResourceName name,
            TypeRegistry<IBlockEvent, BlockEventInfo> eventRegistry,
            Registry<IBlockAttribute> attributeRegistry,
            Registry<IBlockCapability> capabilityRegistry
        )
        {
            var eventHandlers = new Dictionary<Type, GenericBlockEventDelegate>();
            foreach (var (evtType, handlers) in _eventHandlers)
            {
                var defaultHandler = eventRegistry[evtType].DefaultHandler;
                GenericBlockEventDelegate GetDelegate(int i)
                {
                    return (evt, dataContainer) =>
                    {
                        if (i >= handlers.Count)
                            return defaultHandler(evt, dataContainer);
                        return handlers[i](evt, dataContainer, () => GetDelegate(i + 1)(evt, dataContainer));
                    };
                }
                eventHandlers[evtType] = GetDelegate(0);
            }
            foreach (var (evtType, info) in eventRegistry)
                eventHandlers.TryAdd(evtType, info.DefaultHandler);

            var attributeSuppliers = new Dictionary<IBlockAttribute, GenericBlockAttributeDelegate>();
            foreach (var (attribute, suppliers) in _attributeSuppliers)
            {
                GenericBlockAttributeDelegate GetDelegate(int i)
                {
                    return (context, dataContainer) =>
                    {
                        if (i >= suppliers.Count)
                            return attribute.GenericDefaultValueDelegate(context);
                        return suppliers[i](context, dataContainer, () => GetDelegate(i + 1)(context, dataContainer));
                    };
                }
                attributeSuppliers[attribute] = GetDelegate(0);
            }
            foreach (var attribute in attributeRegistry.Values)
                attributeSuppliers.TryAdd(attribute, (context, _) => attribute.GenericDefaultValueDelegate(context));

            var capabilitySuppliers = new Dictionary<IBlockCapability, GenericBlockCapabilityDelegate>();
            foreach (var (capability, suppliers) in _capabilitySuppliers)
            {
                GenericBlockCapabilityDelegate GetDelegate(int i)
                {
                    return (context, dataContainer) =>
                    {
                        if (i >= suppliers.Count)
                            return capability.GenericDefaultValueDelegate(context);
                        return suppliers[i](context, dataContainer, () => GetDelegate(i + 1)(context, dataContainer));
                    };
                }
                capabilitySuppliers[capability] = GetDelegate(0);
            }
            foreach (var capability in capabilityRegistry.Values)
                capabilitySuppliers.TryAdd(capability, (context, _) => capability.GenericDefaultValueDelegate(context));
            
            DataContainer CreateData()
            {
                var container = new DataContainer();
                foreach (var initializer in _dataInitializers)
                    initializer(container);
                return container;
            }
            
            return new Block(
                name,
                eventHandlers, attributeSuppliers, capabilitySuppliers,
                _dataHandles.Count > 0 ? CreateData : CreateNullData,
                new DataContainerSerdes(_serializedDataHandles)
            );
        }
    }
}