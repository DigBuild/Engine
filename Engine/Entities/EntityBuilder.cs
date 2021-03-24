using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public delegate ref TOut RefFunc<in TIn, TOut>(TIn input);
    
    internal delegate object GenericEntityEventDelegate(IEntityContext context, DataContainer dataContainer, IEntityEvent evt);
    internal delegate object GenericEntityAttributeDelegate(IEntityContext context, DataContainer dataContainer);
    internal delegate object GenericEntityCapabilityDelegate(IEntityContext context, DataContainer dataContainer);

    public sealed class EntityBuilder
    {
        private readonly List<IDataHandle> _dataHandles = new();
        private readonly Dictionary<Type, List<EntityEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IEntityAttribute, List<EntityAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IEntityCapability, List<EntityCapabilityDelegate>> _capabilitySuppliers = new();
        private readonly List<Action<DataContainer>> _dataInitializers = new();

        public DataHandle<TData> Add<TData>()
            where TData : class, new()
        {
            var handle = new DataHandle<TData>();
            _dataHandles.Add(handle);
            return handle;
        }

        public void Attach(IEntityBehavior behavior) => AttachLast(behavior);
        public void Attach<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, new()
            => AttachLast(behavior, data);
        public void Attach<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, new()
            => AttachLast(behavior, data, adapter);
        
        public void AttachLast(IEntityBehavior behavior)
        {
            var builder = new EntityBehaviorBuilder<object, object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, false);
        }
        public void AttachLast<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(container.Get(data)));
        }

        public void AttachLast<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(adapter(container.Get(data))));
        }

        public void AttachFirst(IEntityBehavior<object> behavior)
        {
            var builder = new EntityBehaviorBuilder<object, object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, true);
        }
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, true);
            
            _dataInitializers.Insert(0, container => behavior.Init(container.Get(data)));
        }
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IEntityBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this entity.", nameof(data));

            var builder = new EntityBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container.Get(data)));
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
            ExtendedTypeRegistry<IEntityEvent, EntityEventInfo> eventRegistry,
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
                    return (context, dataContainer, evt) =>
                    {
                        if (i >= handlers.Count)
                            return defaultHandler(context, dataContainer, evt);
                        return handlers[i](context, dataContainer, evt, () => GetDelegate(i + 1)(context, dataContainer, evt));
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
                attributeSuppliers.TryAdd(attribute, (context, container) => attribute.GenericDefaultValueDelegate(context));

            var capabilitySuppliers = new Dictionary<IEntityCapability, GenericEntityCapabilityDelegate>();
            foreach (var (capability, suppliers) in _capabilitySuppliers)
            {
                GenericEntityCapabilityDelegate GetDelegate(int i)
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
                capabilitySuppliers.TryAdd(capability, (context, container) => capability.GenericDefaultValueDelegate(context));

            void InitializeData(DataContainer container)
            {
                foreach (var initializer in _dataInitializers)
                    initializer(container);
            }

            return new Entity(name, eventHandlers, attributeSuppliers, capabilitySuppliers, InitializeData);
        }
    }
}