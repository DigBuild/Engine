﻿using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public delegate ref TOut RefFunc<in TIn, TOut>(TIn input);
    
    internal delegate object GenericEntityEventDelegate(IEntityEvent evt);
    internal delegate object GenericEntityAttributeDelegate(IReadOnlyEntityInstance instance);
    internal delegate object GenericEntityCapabilityDelegate(EntityInstance instance);
    
    /// <summary>
    /// An entity builder.
    /// </summary>
    public sealed class EntityBuilder
    {
        private static readonly Func<DataContainer?> CreateNullData = () => null;

        private readonly List<IDataHandle> _dataHandles = new();
        private readonly Dictionary<IDataHandle, (ResourceName Name, ISerdes<IData> Serdes)> _serializedDataHandles = new();
        private readonly Dictionary<Type, List<EntityEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IEntityAttribute, List<EntityAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IEntityCapability, List<EntityCapabilityDelegate>> _capabilitySuppliers = new();
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
        public void Attach(IEntityBehavior behavior) => AttachLast(behavior);
        /// <summary>
        /// Attaches a behavior.
        /// </summary>
        /// <typeparam name="TReadOnlyContract">The read-only contract type</typeparam>
        /// <typeparam name="TContract">The contract type</typeparam>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="behavior">The behavior</param>
        /// <param name="data">The data</param>
        public void Attach<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
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
        public void Attach<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
            => AttachLast(behavior, data, adapter);
        
        /// <summary>
        /// Attaches a behavior without a contract at the end of the chain.
        /// </summary>
        /// <param name="behavior">The behavior</param>
        public void AttachLast(IEntityBehavior behavior)
        {
            var builder = new EntityBehaviorBuilder<object, object>(_ => null!);
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
        public void AttachLast<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => container?.Get(data)!);
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
        public void AttachLast<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container?.Get(data)!));
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(adapter(container.Get(data))));
        }
        
        /// <summary>
        /// Attaches a behavior without a contract at the start of the chain.
        /// </summary>
        /// <param name="behavior">The behavior</param>
        public void AttachFirst(IEntityBehavior<object> behavior)
        {
            var builder = new EntityBehaviorBuilder<object, object>(_ => null!);
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
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => container?.Get(data)!);
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
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container?.Get(data)!));
            behavior.Build(builder);
            Attach(builder, true);
            
            _dataInitializers.Insert(0, container => behavior.Init(adapter(container.Get(data))));
        }

        private void Attach(IEntityBehaviorBuilder builder, bool prepend)
        {
            foreach (var (type, list) in builder.EventHandlers)
            {
                if (!_eventHandlers.TryGetValue(type, out var entityList))
                    _eventHandlers[type] = entityList = new List<EntityEventDelegate>();
                entityList.InsertRange(prepend ? 0 : entityList.Count, list);
            }
            foreach (var (attribute, list) in builder.AttributeSuppliers)
            {
                if (!_attributeSuppliers.TryGetValue(attribute, out var entityList))
                    _attributeSuppliers[attribute] = entityList = new List<EntityAttributeDelegate>();
                entityList.InsertRange(prepend ? 0 : entityList.Count, list);
            }
            foreach (var (capability, list) in builder.CapabilitySuppliers)
            {
                if (!_capabilitySuppliers.TryGetValue(capability, out var entityList))
                    _capabilitySuppliers[capability] = entityList = new List<EntityCapabilityDelegate>();
                entityList.InsertRange(prepend ? 0 : entityList.Count, list);
            }
        }

        internal Entity Build(
            ResourceName name,
            TypeRegistry<IEntityEvent, EntityEventInfo> eventRegistry,
            Registry<IEntityAttribute> attributeRegistry,
            Registry<IEntityCapability> capabilityRegistry
        )
        {
            var eventHandlers = new Dictionary<Type, GenericEntityEventDelegate>();
            foreach (var (evtType, handlers) in _eventHandlers)
            {
                var defaultHandler = eventRegistry[evtType].DefaultHandler;
                GenericEntityEventDelegate GetDelegate(int i)
                {
                    return evt =>
                    {
                        if (i >= handlers.Count)
                            return defaultHandler(evt);
                        return handlers[i](evt, () => GetDelegate(i + 1)(evt));
                    };
                }
                eventHandlers[evtType] = GetDelegate(0);
            }
            foreach (var (evtType, info) in eventRegistry)
                eventHandlers.TryAdd(evtType, info.DefaultHandler);

            var attributeSuppliers = new Dictionary<IEntityAttribute, GenericEntityAttributeDelegate>();
            foreach (var (attribute, suppliers) in _attributeSuppliers)
            {
                GenericEntityAttributeDelegate GetDelegate(int i)
                {
                    return instance =>
                    {
                        if (i >= suppliers.Count)
                            return attribute.GenericDefaultValueDelegate(instance);
                        return suppliers[i](instance, () => GetDelegate(i + 1)(instance));
                    };
                }
                attributeSuppliers[attribute] = GetDelegate(0);
            }
            foreach (var attribute in attributeRegistry.Values)
                attributeSuppliers.TryAdd(attribute, instance => attribute.GenericDefaultValueDelegate(instance));

            var capabilitySuppliers = new Dictionary<IEntityCapability, GenericEntityCapabilityDelegate>();
            foreach (var (capability, suppliers) in _capabilitySuppliers)
            {
                GenericEntityCapabilityDelegate GetDelegate(int i)
                {
                    return instance =>
                    {
                        if (i >= suppliers.Count)
                            return capability.GenericDefaultValueDelegate(instance);
                        return suppliers[i](instance, () => GetDelegate(i + 1)(instance));
                    };
                }
                capabilitySuppliers[capability] = GetDelegate(0);
            }
            foreach (var capability in capabilityRegistry.Values)
                capabilitySuppliers.TryAdd(capability, instance => capability.GenericDefaultValueDelegate(instance));
            
            DataContainer CreateData()
            {
                var container = new DataContainer();
                foreach (var initializer in _dataInitializers)
                    initializer(container);
                return container;
            }

            return new Entity(
                name,
                eventHandlers, attributeSuppliers, capabilitySuppliers,
                _dataHandles.Count > 0 ? CreateData : CreateNullData,
                new DataContainerSerdes(_serializedDataHandles)
            );
        }
    }
}