using System;
using System.Collections.Generic;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items
{
    public delegate void ItemEventDelegate<in TData, in TEvent>(TEvent evt, TData data, Action next)
        where TEvent : IItemEvent;
    public delegate TResult ItemEventDelegate<in TData, in TEvent, TResult>(TEvent evt, TData data, Func<TResult> next)
        where TEvent : IItemEvent<TResult>;
    
    public delegate T ItemAttributeDelegate<in TData, T>(IReadOnlyItemContext context, TData data, Func<T> next);
    public delegate T ItemCapabilityDelegate<in TData, T>(IItemContext context, TData data, Func<T> next);
    
    internal delegate object ItemEventDelegate(IItemEvent evt, DataContainer dataContainer, Func<object> next);
    internal delegate object ItemAttributeDelegate(IReadOnlyItemContext context, DataContainer dataContainer, Func<object> next);
    internal delegate object ItemCapabilityDelegate(IItemContext context, DataContainer dataContainer, Func<object> next);

    public interface IItemBehaviorBuilder
    {
        internal Dictionary<Type, List<ItemEventDelegate>> EventHandlers { get; }
        internal Dictionary<IItemAttribute, List<ItemAttributeDelegate>> AttributeSuppliers { get; }
        internal Dictionary<IItemCapability, List<ItemCapabilityDelegate>> CapabilitySuppliers { get; }
    }

    public interface IItemBehaviorBuilder<out TReadOnlyData, out TData> : IItemBehaviorBuilder
        where TData : TReadOnlyData
    {
        void Subscribe<TEvent>(ItemEventDelegate<TData, TEvent> del)
            where TEvent : IItemEvent;
        void Subscribe<TEvent, TResult>(ItemEventDelegate<TData, TEvent, TResult> del)
            where TEvent : IItemEvent<TResult>;
        
        void Add<T>(ItemAttribute<T> attribute, ItemAttributeDelegate<TReadOnlyData, T> supplier);
        void Add<T>(ItemCapability<T> capability, ItemCapabilityDelegate<TData, T> supplier);
    }

    public sealed class ItemBehaviorBuilder<TReadOnlyData, TData> : IItemBehaviorBuilder<TReadOnlyData, TData>
        where TData : TReadOnlyData
    {
        private readonly Dictionary<Type, List<ItemEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IItemAttribute, List<ItemAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IItemCapability, List<ItemCapabilityDelegate>> _capabilitySuppliers = new();

        Dictionary<Type, List<ItemEventDelegate>> IItemBehaviorBuilder.EventHandlers => _eventHandlers;
        Dictionary<IItemAttribute, List<ItemAttributeDelegate>> IItemBehaviorBuilder.AttributeSuppliers => _attributeSuppliers;
        Dictionary<IItemCapability, List<ItemCapabilityDelegate>> IItemBehaviorBuilder.CapabilitySuppliers => _capabilitySuppliers;

        private readonly Func<DataContainer, TData> _dataGetter;

        internal ItemBehaviorBuilder(Func<DataContainer, TData> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        void IItemBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent>(ItemEventDelegate<TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<ItemEventDelegate>();
            list.Add((evt, dataContainer, next) =>
            {
                del((TEvent) evt, _dataGetter(dataContainer), () => next());
                return null!;
            });
        }

        void IItemBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent, TResult>(ItemEventDelegate<TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<ItemEventDelegate>();
            list.Add((evt, dataContainer, next) =>
            {
                return del((TEvent) evt, _dataGetter(dataContainer), () => (TResult) next())!;
            });
        }

        public void Add<T>(ItemAttribute<T> attribute, ItemAttributeDelegate<TReadOnlyData, T> supplier)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var list))
                _attributeSuppliers[attribute] = list = new List<ItemAttributeDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), () => (T) next())!;
            });
        }

        public void Add<T>(ItemCapability<T> capability, ItemCapabilityDelegate<TData, T> supplier)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var list))
                _capabilitySuppliers[capability] = list = new List<ItemCapabilityDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), () => (T) next())!;
            });
        }
    }
}