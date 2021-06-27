using System;
using System.Collections.Generic;
using System.Linq;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public delegate ref TOut RefFunc<in TIn, TOut>(TIn input);
    
    internal delegate object GenericItemEventDelegate(IItemEvent evt);
    internal delegate object GenericItemAttributeDelegate(IReadOnlyItemInstance instance);
    internal delegate object GenericItemCapabilityDelegate(ItemInstance instance);

    public sealed class ItemBuilder
    {
        private readonly List<IDataHandle> _dataHandles = new();
        private readonly Dictionary<Type, List<ItemEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IItemAttribute, List<ItemAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IItemCapability, List<ItemCapabilityDelegate>> _capabilitySuppliers = new();
        private readonly List<Action<DataContainer>> _dataInitializers = new();
        private readonly List<Func<DataContainer, DataContainer, bool>> _equalityChecks = new();

        public DataHandle<TData> Add<TData>()
            where TData : class, IData<TData>, new()
        {
            var handle = new DataHandle<TData>(() => new TData());
            _dataHandles.Add(handle);
            return handle;
        }

        public void Attach(IItemBehavior behavior) => AttachLast(behavior);
        public void Attach<TReadOnlyContract, TContract, TData>(IItemBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
            => AttachLast(behavior, data);
        public void Attach<TReadOnlyContract, TContract, TData>(IItemBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
            => AttachLast(behavior, data, adapter);
        
        public void AttachLast(IItemBehavior behavior)
        {
            var builder = new ItemBehaviorBuilder<object, object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, false);
        }
        public void AttachLast<TReadOnlyContract, TContract, TData>(IItemBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TReadOnlyContract, TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(container.Get(data)));
            _equalityChecks.Add((first, second) => behavior.Equals(first.Get(data), second.Get(data)));
        }

        public void AttachLast<TReadOnlyContract, TContract, TData>(IItemBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, false);
            
            _dataInitializers.Add(container => behavior.Init(adapter(container.Get(data))));
            _equalityChecks.Add((first, second) => behavior.Equals(adapter(first.Get(data)), adapter(second.Get(data))));
        }

        public void AttachFirst(IItemBehavior behavior)
        {
            var builder = new ItemBehaviorBuilder<object, object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, true);
        }
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IItemBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data)
            where TContract : TReadOnlyContract
            where TData : class, TContract, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TReadOnlyContract, TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, true);

            _dataInitializers.Insert(0, container => behavior.Init(container.Get(data)));
            _equalityChecks.Insert(0, (first, second) => behavior.Equals(first.Get(data), second.Get(data)));
        }
        public void AttachFirst<TReadOnlyContract, TContract, TData>(IItemBehavior<TReadOnlyContract, TContract> behavior, DataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TContract : TReadOnlyContract
            where TData : class, IData<TData>, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this Item.", nameof(data));

            var builder = new ItemBehaviorBuilder<TReadOnlyContract, TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, true);

            _dataInitializers.Insert(0, container => behavior.Init(adapter(container.Get(data))));
            _equalityChecks.Insert(0, (first, second) => behavior.Equals(adapter(first.Get(data)), adapter(second.Get(data))));
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

            var attributeSuppliers = new Dictionary<IItemAttribute, GenericItemAttributeDelegate>();
            foreach (var (attribute, suppliers) in _attributeSuppliers)
            {
                GenericItemAttributeDelegate GetDelegate(int i)
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

            var capabilitySuppliers = new Dictionary<IItemCapability, GenericItemCapabilityDelegate>();
            foreach (var (capability, suppliers) in _capabilitySuppliers)
            {
                GenericItemCapabilityDelegate GetDelegate(int i)
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

            void InitializeData(DataContainer container)
            {
                foreach (var initializer in _dataInitializers)
                    initializer(container);
            }

            bool TestEquals(DataContainer first, DataContainer second)
            {
                return _equalityChecks.All(check => check(first, second));
            }

            return new Item(name, eventHandlers, attributeSuppliers, capabilitySuppliers, InitializeData, TestEquals);
        }
    }
}