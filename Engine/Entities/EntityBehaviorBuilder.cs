using System;
using System.Collections.Generic;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Entities
{
    public delegate void EntityEventDelegate<in TData, in TEvent>(TEvent evt, TData data, Action next)
        where TEvent : IEntityEvent;
    public delegate TResult EntityEventDelegate<in TData, in TEvent, TResult>(TEvent evt, TData data, Func<TResult> next)
        where TEvent : IEntityEvent<TResult>;
    
    public delegate T EntityAttributeDelegate<in TData, T>(IReadOnlyEntityInstance instance, TData data, Func<T> next);
    public delegate T EntityCapabilityDelegate<in TData, T>(EntityInstance instance, TData data, Func<T> next);
    
    internal delegate object EntityEventDelegate(IEntityEvent evt, Func<object> next);
    internal delegate object EntityAttributeDelegate(IReadOnlyEntityInstance instance, Func<object> next);
    internal delegate object EntityCapabilityDelegate(EntityInstance instance, Func<object> next);

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

        private readonly Func<DataContainer?, TData> _dataGetter;

        internal EntityBehaviorBuilder(Func<DataContainer?, TData> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        void IEntityBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent>(EntityEventDelegate<TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<EntityEventDelegate>();
            list.Add((evt, next) =>
            {
                del((TEvent) evt, _dataGetter(evt.Entity.DataContainer), () => next());
                return null!;
            });
        }

        void IEntityBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent, TResult>(EntityEventDelegate<TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<EntityEventDelegate>();
            list.Add((evt, next) =>
            {
                return del((TEvent) evt, _dataGetter(evt.Entity.DataContainer), () => (TResult) next())!;
            });
        }

        public void Add<T>(EntityAttribute<T> attribute, EntityAttributeDelegate<TReadOnlyData, T> supplier)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var list))
                _attributeSuppliers[attribute] = list = new List<EntityAttributeDelegate>();
            list.Add((instance, next) =>
            {
                return supplier(instance, _dataGetter(instance.DataContainer), () => (T) next())!;
            });
        }

        public void Add<T>(EntityCapability<T> capability, EntityCapabilityDelegate<TData, T> supplier)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var list))
                _capabilitySuppliers[capability] = list = new List<EntityCapabilityDelegate>();
            list.Add((instance, next) =>
            {
                return supplier(instance, _dataGetter(instance.DataContainer), () => (T) next())!;
            });
        }
    }
}