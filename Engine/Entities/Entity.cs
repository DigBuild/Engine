using System;
using System.Collections.Generic;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An entity.
    /// </summary>
    public sealed class Entity
    {
        private readonly IReadOnlyDictionary<Type, GenericEntityEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IEntityAttribute, GenericEntityAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IEntityCapability, GenericEntityCapabilityDelegate> _capabilitySuppliers;
        private readonly Func<DataContainer?> _dataFactory;

        internal ISerdes<DataContainer?> DataSerdes { get; }

        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }

        internal Entity(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericEntityEventDelegate> eventHandlers,
            IReadOnlyDictionary<IEntityAttribute, GenericEntityAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IEntityCapability, GenericEntityCapabilityDelegate> capabilitySuppliers,
            Func<DataContainer?> dataFactory,
            ISerdes<DataContainer?> dataSerdes
        )
        {
            _eventHandlers = eventHandlers;
            _attributeSuppliers = attributeSuppliers;
            _capabilitySuppliers = capabilitySuppliers;
            _dataFactory = dataFactory;
            DataSerdes = dataSerdes;
            Name = name;
        }
        
        /// <summary>
        /// Posts an event to the entity.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="evt">The event</param>
        public void Post<TEvent>(TEvent evt) where TEvent : IEntityEvent
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(evt);
        }
        
        /// <summary>
        /// Posts an event with a return value to the entity.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="TOut">The return type</typeparam>
        /// <param name="evt">The event</param>
        /// <returns>The return value</returns>
        public TOut Post<TEvent, TOut>(TEvent evt) where TEvent : IEntityEvent<TOut>
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(evt);
        }
        
        /// <summary>
        /// Gets the value of an attribute for a specific entity instance.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public TAttrib Get<TAttrib>(IReadOnlyEntityInstance instance, EntityAttribute<TAttrib> attribute)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered attribute: {attribute}", nameof(attribute));
            return (TAttrib) supplier(instance);
        }
        
        /// <summary>
        /// Gets the value of a capability for a specific entity instance.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="capability">The capability</param>
        /// <returns>The value</returns>
        public TCap Get<TCap>(EntityInstance instance, EntityCapability<TCap> capability)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered capability: {capability}", nameof(capability));
            return (TCap) supplier(instance);
        }

        internal DataContainer? CreateDataContainer()
        {
            return _dataFactory();
        }

        public override string ToString()
        {
            return $"Entity({Name})";
        }
    }
    
    /// <summary>
    /// Registry extensions for entities.
    /// </summary>
    public static class EntityRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new entity.
        /// </summary>
        /// <param name="registry">The entity registry</param>
        /// <param name="name">The name</param>
        /// <param name="buildActions">The build actions</param>
        /// <returns>The entity</returns>
        public static Entity Register(this IRegistryBuilder<Entity> registry, ResourceName name, params Action<EntityBuilder>[] buildActions)
        {
            var builder = new EntityBuilder();
            foreach (var action in buildActions)
                action(builder);
            DigBuildEngine.EventBus.Post(new EntityBuildingEvent(name, builder));
            return registry.Add(name, builder.Build(name, DigBuildEngine.EntityEvents, DigBuildEngine.EntityAttributes, DigBuildEngine.EntityCapabilities));
        }
    }
}