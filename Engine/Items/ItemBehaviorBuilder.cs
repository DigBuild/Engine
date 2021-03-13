using System;
using System.Collections.Generic;

namespace DigBuild.Engine.Items
{
    public delegate void ItemEventDelegate<in TContext, in TData, in TEvent>(TContext context, TData data, TEvent evt, Action next)
        where TContext : IItemContext
        where TEvent : IItemEvent<TContext>;
    public delegate TResult ItemEventDelegate<in TContext, in TData, in TEvent, TResult>(TContext context, TData data, TEvent evt, Func<TResult> next)
        where TContext : IItemContext
        where TEvent : IItemEvent<TContext, TResult>;
    
    public delegate T ItemAttributeDelegate<in TData, T>(IItemContext context, TData data, ItemAttribute<T> attribute, Func<T> next);
    public delegate T ItemCapabilityDelegate<in TData, T>(IItemContext context, TData data, ItemCapability<T> capability, Func<T> next);
    
    internal delegate object ItemEventDelegate(IItemContext context, ItemDataContainer dataContainer, IItemEvent evt, Func<object> next);
    internal delegate object ItemAttributeDelegate(IItemContext context, ItemDataContainer dataContainer, Func<object> next);
    internal delegate object ItemCapabilityDelegate(IItemContext context, ItemDataContainer dataContainer, Func<object> next);

    public interface IItemBehaviorBuilder
    {
        internal Dictionary<Type, List<ItemEventDelegate>> EventHandlers { get; }
        internal Dictionary<IItemAttribute, List<ItemAttributeDelegate>> AttributeSuppliers { get; }
        internal Dictionary<IItemCapability, List<ItemCapabilityDelegate>> CapabilitySuppliers { get; }
    }

    public interface IItemBehaviorBuilder<out TData> : IItemBehaviorBuilder
    {
        void Subscribe<TContext, TEvent>(ItemEventDelegate<TContext, TData, TEvent> del)
            where TContext : IItemContext
            where TEvent : IItemEvent<TContext>;
        void Subscribe<TContext, TEvent, TResult>(ItemEventDelegate<TContext, TData, TEvent, TResult> del)
            where TContext : IItemContext
            where TEvent : IItemEvent<TContext, TResult>;
        
        void Add<T>(ItemAttribute<T> attribute, ItemAttributeDelegate<TData, T> supplier);
        void Add<T>(ItemCapability<T> capability, ItemCapabilityDelegate<TData, T> supplier);
    }

    public sealed class ItemBehaviorBuilder<TData> : IItemBehaviorBuilder<TData>
    {
        private readonly Dictionary<Type, List<ItemEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IItemAttribute, List<ItemAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IItemCapability, List<ItemCapabilityDelegate>> _capabilitySuppliers = new();

        Dictionary<Type, List<ItemEventDelegate>> IItemBehaviorBuilder.EventHandlers => _eventHandlers;
        Dictionary<IItemAttribute, List<ItemAttributeDelegate>> IItemBehaviorBuilder.AttributeSuppliers => _attributeSuppliers;
        Dictionary<IItemCapability, List<ItemCapabilityDelegate>> IItemBehaviorBuilder.CapabilitySuppliers => _capabilitySuppliers;

        private readonly Func<ItemDataContainer, TData> _dataGetter;

        internal ItemBehaviorBuilder(Func<ItemDataContainer, TData> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        void IItemBehaviorBuilder<TData>.Subscribe<TContext, TEvent>(ItemEventDelegate<TContext, TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<ItemEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                del((TContext) context, _dataGetter(dataContainer), (TEvent) evt, () => next());
                return null!;
            });
        }

        void IItemBehaviorBuilder<TData>.Subscribe<TContext, TEvent, TResult>(ItemEventDelegate<TContext, TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<ItemEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                return del((TContext) context, _dataGetter(dataContainer), (TEvent) evt, () => (TResult) next())!;
            });
        }

        public void Add<T>(ItemAttribute<T> attribute, ItemAttributeDelegate<TData, T> supplier)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var list))
                _attributeSuppliers[attribute] = list = new List<ItemAttributeDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), attribute, () => (T) next())!;
            });
        }

        public void Add<T>(ItemCapability<T> capability, ItemCapabilityDelegate<TData, T> supplier)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var list))
                _capabilitySuppliers[capability] = list = new List<ItemCapabilityDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), capability, () => (T) next())!;
            });
        }
    }
}