﻿using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public sealed class Entity
    {
        private readonly IReadOnlyDictionary<Type, GenericEntityEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IEntityAttribute, GenericEntityAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IEntityCapability, GenericEntityCapabilityDelegate> _capabilitySuppliers;
        private readonly Action<DataContainer> _dataInitializer;

        public ResourceName Name { get; }

        internal Entity(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericEntityEventDelegate> eventHandlers,
            IReadOnlyDictionary<IEntityAttribute, GenericEntityAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IEntityCapability, GenericEntityCapabilityDelegate> capabilitySuppliers,
            Action<DataContainer> dataInitializer
        )
        {
            _eventHandlers = eventHandlers;
            _attributeSuppliers = attributeSuppliers;
            _capabilitySuppliers = capabilitySuppliers;
            _dataInitializer = dataInitializer;
            Name = name;
        }

        public void Post<TEvent>(TEvent evt) where TEvent : IEntityEvent
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(evt);
        }

        public TOut Post<TEvent, TOut>(TEvent evt) where TEvent : IEntityEvent<TOut>
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(evt);
        }

        public TAttrib Get<TAttrib>(IReadOnlyEntityInstance instance, EntityAttribute<TAttrib> attribute)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered attribute: {attribute}", nameof(attribute));
            return (TAttrib) supplier(instance);
        }

        public TCap Get<TCap>(EntityInstance instance, EntityCapability<TCap> capability)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered capability: {capability}", nameof(capability));
            return (TCap) supplier(instance);
        }

        internal DataContainer CreateDataContainer()
        {
            var container = new DataContainer();
            _dataInitializer(container);
            return container;
        }

        public override string ToString()
        {
            return $"Entity({Name})";
        }
    }

    public static class EntityRegistryBuilderExtensions
    {
        public static ExtendedTypeRegistry<IEntityEvent, EntityEventInfo> EventRegistry = null!;
        public static Registry<IEntityAttribute> EntityAttributes = null!;
        public static Registry<IEntityCapability> EntityCapabilities = null!;

        public static Entity Create(this IRegistryBuilder<Entity> registry, ResourceName name, params Action<EntityBuilder>[] buildActions)
        {
            var builder = new EntityBuilder();
            foreach (var action in buildActions)
                action(builder);
            return registry.Add(name, builder.Build(name, EventRegistry, EntityAttributes, EntityCapabilities));
        }
    }
}