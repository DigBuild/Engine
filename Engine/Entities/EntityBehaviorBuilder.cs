using System;
using System.Collections.Generic;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Entities
{
    public delegate void EntityEventDelegate<in TData, in TEvent>(IEntityContext context, TData data, TEvent evt, Action next)
        where TEvent : IEntityEvent;
    public delegate TResult EntityEventDelegate<in TData, in TEvent, TResult>(IEntityContext context, TData data, TEvent evt, Func<TResult> next)
        where TEvent : IEntityEvent<TResult>;
    
    public delegate T EntityAttributeDelegate<in TData, T>(IReadOnlyEntityContext context, TData data, EntityAttribute<T> attribute, Func<T> next);
    public delegate T EntityCapabilityDelegate<in TData, T>(IEntityContext context, TData data, EntityCapability<T> capability, Func<T> next);
    
    internal delegate object EntityEventDelegate(IEntityContext context, DataContainer dataContainer, IEntityEvent evt, Func<object> next);
    internal delegate object EntityAttributeDelegate(IReadOnlyEntityContext context, DataContainer dataContainer, Func<object> next);
    internal delegate object EntityCapabilityDelegate(IEntityContext context, DataContainer dataContainer, Func<object> next);

    public interface IEntityBehaviorBuilder
    {
        internal Dictionary<Type, List<EntityEventDelegate>> EventHandlers { get; }
        internal Dictionary<IEntityAttribute, List<EntityAttributeDelegate>> AttributeSuppliers { get; }
        internal Dictionary<IEntityCapability, List<EntityCapabilityDelegate>> CapabilitySuppliers { get; }
    }

    public interface IEntityBehaviorBuilder<out TReadOnlyData, out TData> : IEntityBehaviorBuilder
        where TData : TReadOnlyData
    {
        void Subscribe<TEvent>(EntityEventDelegate<TData, TEvent> del)
            where TEvent : IEntityEvent;
        void Subscribe<TEvent, TResult>(EntityEventDelegate<TData, TEvent, TResult> del)
            where TEvent : IEntityEvent<TResult>;
        
        void Add<T>(EntityAttribute<T> attribute, EntityAttributeDelegate<TReadOnlyData, T> supplier);
        void Add<T>(EntityCapability<T> capability, EntityCapabilityDelegate<TData, T> supplier);
    }

    public sealed class EntityBehaviorBuilder<TReadOnlyData, TData> : IEntityBehaviorBuilder<TReadOnlyData, TData>
        where TData : TReadOnlyData
    {
        private readonly Dictionary<Type, List<EntityEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IEntityAttribute, List<EntityAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IEntityCapability, List<EntityCapabilityDelegate>> _capabilitySuppliers = new();

        Dictionary<Type, List<EntityEventDelegate>> IEntityBehaviorBuilder.EventHandlers => _eventHandlers;
        Dictionary<IEntityAttribute, List<EntityAttributeDelegate>> IEntityBehaviorBuilder.AttributeSuppliers => _attributeSuppliers;
        Dictionary<IEntityCapability, List<EntityCapabilityDelegate>> IEntityBehaviorBuilder.CapabilitySuppliers => _capabilitySuppliers;

        private readonly Func<DataContainer, TData> _dataGetter;

        internal EntityBehaviorBuilder(Func<DataContainer, TData> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        void IEntityBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent>(EntityEventDelegate<TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<EntityEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                del(context, _dataGetter(dataContainer), (TEvent) evt, () => next());
                return null!;
            });
        }

        void IEntityBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent, TResult>(EntityEventDelegate<TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<EntityEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                return del(context, _dataGetter(dataContainer), (TEvent) evt, () => (TResult) next())!;
            });
        }

        public void Add<T>(EntityAttribute<T> attribute, EntityAttributeDelegate<TReadOnlyData, T> supplier)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var list))
                _attributeSuppliers[attribute] = list = new List<EntityAttributeDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), attribute, () => (T) next())!;
            });
        }

        public void Add<T>(EntityCapability<T> capability, EntityCapabilityDelegate<TData, T> supplier)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var list))
                _capabilitySuppliers[capability] = list = new List<EntityCapabilityDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), capability, () => (T) next())!;
            });
        }
    }
}