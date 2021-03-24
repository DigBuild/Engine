using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public delegate ref TOut RefFunc<in TIn, TOut>(TIn input);
    
    internal delegate object GenericItemEventDelegate(IItemContext context, DataContainer dataContainer, IItemEvent evt);
    internal delegate object GenericItemAttributeDelegate(IItemContext context, DataContainer dataContainer);
    internal delegate object GenericItemCapabilityDelegate(IItemContext context, DataContainer dataContainer);

    public sealed class ItemBuilder
    {
        private readonly List<IDataHandle> _dataHandles = new();
        private readonly Dictionary<Type, List<ItemEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IItemAttribute, List<ItemAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IItemCapability, List<ItemCapabilityDelegate>> _capabilitySuppliers = new();

        public DataHandle<TData> Add<TData>()
            where TData : class, new()
        {
            var handle = new DataHandle<TData>();
            _dataHandles.Add(handle);
            return handle;
        }

        public void Attach(IItemBehavior<object> behavior) => AttachLast(behavior);
        public void Attach<TContract, TData>(IItemBehavior<TContract> behavior, DataHandle<TData> data)
            where TData : class, TContract, new()
            => AttachLast(behavior, data);
        public void Attach<TContract, TData>(IItemBehavior<TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TData : class, new()
            => AttachLast(behavior, data, adapter);
        
        public void AttachLast(IItemBehavior<object> behavior)
        {
            var builder = new ItemBehaviorBuilder<object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, false);
        }
        public void AttachLast<TContract, TData>(IItemBehavior<TContract> behavior, DataHandle<TData> data)
            where TData : class, TContract, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, false);
        }

        public void AttachLast<TContract, TData>(IItemBehavior<TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TData : class, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, false);
        }

        public void AttachFirst(IItemBehavior<object> behavior)
        {
            var builder = new ItemBehaviorBuilder<object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, true);
        }
        public void AttachFirst<TContract, TData>(IItemBehavior<TContract> behavior, DataHandle<TData> data)
            where TData : class, TContract, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, true);
        }
        public void AttachFirst<TContract, TData>(IItemBehavior<TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TData : class, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, true);
        }

        private void Attach(IItemBehaviorBuilder builder, bool prepend)
        {
            foreach (var (type, list) in builder.EventHandlers)
            {
                if (!_eventHandlers.TryGetValue(type, out var itemList))
                    _eventHandlers[type] = itemList = new List<ItemEventDelegate>();
                itemList.InsertRange(prepend ? 0 : itemList.Count, list);
            }
            foreach (var (attribute, list) in builder.AttributeSuppliers)
            {
                if (!_attributeSuppliers.TryGetValue(attribute, out var itemList))
                    _attributeSuppliers[attribute] = itemList = new List<ItemAttributeDelegate>();
                itemList.InsertRange(prepend ? 0 : itemList.Count, list);
            }
            foreach (var (capability, list) in builder.CapabilitySuppliers)
            {
                if (!_capabilitySuppliers.TryGetValue(capability, out var itemList))
                    _capabilitySuppliers[capability] = itemList = new List<ItemCapabilityDelegate>();
                itemList.InsertRange(prepend ? 0 : itemList.Count, list);
            }
        }

        internal Item Build(
            ResourceName name,
            ExtendedTypeRegistry<IItemEvent, ItemEventInfo> eventRegistry,
            Registry<IItemAttribute> attributeRegistry,
            Registry<IItemCapability> capabilityRegistry
        )
        {
            var eventHandlers = new Dictionary<Type, GenericItemEventDelegate>();
            foreach (var (evtType, handlers) in _eventHandlers)
            {
                var defaultHandler = eventRegistry[evtType].DefaultHandler;
                GenericItemEventDelegate GetDelegate(int i)
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

            var attributeSuppliers = new Dictionary<IItemAttribute, GenericItemAttributeDelegate>();
            foreach (var (attribute, suppliers) in _attributeSuppliers)
            {
                GenericItemAttributeDelegate GetDelegate(int i)
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

            var capabilitySuppliers = new Dictionary<IItemCapability, GenericItemCapabilityDelegate>();
            foreach (var (capability, suppliers) in _capabilitySuppliers)
            {
                GenericItemCapabilityDelegate GetDelegate(int i)
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

            return new Item(name, eventHandlers, attributeSuppliers, capabilitySuppliers);
        }
    }
}