using System;
using System.Collections.Generic;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Entities
{
    public delegate void EntityEventDelegate<in TContext, in TData, in TEvent>(TContext context, TData data, TEvent evt, Action next)
        where TContext : IEntityContext
        where TEvent : IEntityEvent<TContext>;
    public delegate TResult EntityEventDelegate<in TContext, in TData, in TEvent, TResult>(TContext context, TData data, TEvent evt, Func<TResult> next)
        where TContext : IEntityContext
        where TEvent : IEntityEvent<TContext, TResult>;
    
    public delegate T EntityAttributeDelegate<in TData, T>(IEntityContext context, TData data, EntityAttribute<T> attribute, Func<T> next);
    public delegate T EntityCapabilityDelegate<in TData, T>(IEntityContext context, TData data, EntityCapability<T> capability, Func<T> next);
    
    internal delegate object EntityEventDelegate(IEntityContext context, DataContainer dataContainer, IEntityEvent evt, Func<object> next);
    internal delegate object EntityAttributeDelegate(IEntityContext context, DataContainer dataContainer, Func<object> next);
    internal delegate object EntityCapabilityDelegate(IEntityContext context, DataContainer dataContainer, Func<object> next);

    public interface IEntityBehaviorBuilder
    {
        internal Dictionary<Type, List<EntityEventDelegate>> EventHandlers { get; }
        internal Dictionary<IEntityAttribute, List<EntityAttributeDelegate>> AttributeSuppliers { get; }
        internal Dictionary<IEntityCapability, List<EntityCapabilityDelegate>> CapabilitySuppliers { get; }
    }

    public interface IEntityBehaviorBuilder<out TData> : IEntityBehaviorBuilder
    {
        void Subscribe<TContext, TEvent>(EntityEventDelegate<TContext, TData, TEvent> del)
            where TContext : IEntityContext
            where TEvent : IEntityEvent<TContext>;
        void Subscribe<TContext, TEvent, TResult>(EntityEventDelegate<TContext, TData, TEvent, TResult> del)
            where TContext : IEntityContext
            where TEvent : IEntityEvent<TContext, TResult>;
        
        void Add<T>(EntityAttribute<T> attribute, EntityAttributeDelegate<TData, T> supplier);
        void Add<T>(EntityCapability<T> capability, EntityCapabilityDelegate<TData, T> supplier);
    }

    public sealed class EntityBehaviorBuilder<TData> : IEntityBehaviorBuilder<TData>
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

        void IEntityBehaviorBuilder<TData>.Subscribe<TContext, TEvent>(EntityEventDelegate<TContext, TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<EntityEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                del((TContext) context, _dataGetter(dataContainer), (TEvent) evt, () => next());
                return null!;
            });
        }

        void IEntityBehaviorBuilder<TData>.Subscribe<TContext, TEvent, TResult>(EntityEventDelegate<TContext, TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<EntityEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                return del((TContext) context, _dataGetter(dataContainer), (TEvent) evt, () => (TResult) next())!;
            });
        }

        public void Add<T>(EntityAttribute<T> attribute, EntityAttributeDelegate<TData, T> supplier)
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