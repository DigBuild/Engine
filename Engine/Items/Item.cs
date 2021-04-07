using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public sealed class Item
    {
        private readonly IReadOnlyDictionary<Type, GenericItemEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IItemAttribute, GenericItemAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IItemCapability, GenericItemCapabilityDelegate> _capabilitySuppliers;
        private readonly Action<DataContainer> _dataInitializer;

        public ResourceName Name { get; }

        internal Item(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericItemEventDelegate> eventHandlers,
            IReadOnlyDictionary<IItemAttribute, GenericItemAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IItemCapability, GenericItemCapabilityDelegate> capabilitySuppliers,
            Action<DataContainer> dataInitializer
        )
        {
            _eventHandlers = eventHandlers;
            _attributeSuppliers = attributeSuppliers;
            _capabilitySuppliers = capabilitySuppliers;
            _dataInitializer = dataInitializer;
            Name = name;
        }

        public void Post<TEvent>(TEvent evt) where TEvent : IItemEvent
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(evt, GetDataContainer(evt));
        }

        public TOut Post<TEvent, TOut>(TEvent evt) where TEvent : IItemEvent<TOut>
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(evt, GetDataContainer(evt));
        }

        public TAttrib Get<TAttrib>(IReadOnlyItemContext context, ItemAttribute<TAttrib> attribute)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered attribute: {attribute}", nameof(attribute));
            return (TAttrib) supplier(context, GetDataContainer(context));
        }

        public TCap Get<TCap>(IItemContext context, ItemCapability<TCap> capability)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered capability: {capability}", nameof(capability));
            return (TCap) supplier(context, GetDataContainer(context));
        }

        internal DataContainer CreateDataContainer()
        {
            var container = new DataContainer();
            _dataInitializer(container);
            return container;
        }
        
        private DataContainer GetDataContainer(IReadOnlyItemContext context)
        {
            return context.Instance.DataContainer;
        }

        public override string ToString()
        {
            return $"Item({Name})";
        }
    }

    public static class ItemRegistryBuilderExtensions
    {
        public static ExtendedTypeRegistry<IItemEvent, ItemEventInfo> EventRegistry = null!;
        public static Registry<IItemAttribute> ItemAttributes = null!;
        public static Registry<IItemCapability> ItemCapabilities = null!;

        public static Item Create(this IRegistryBuilder<Item> registry, ResourceName name, params Action<ItemBuilder>[] buildActions)
        {
            var builder = new ItemBuilder();
            foreach (var action in buildActions)
                action(builder);
            return registry.Add(name, builder.Build(name, EventRegistry, ItemAttributes, ItemCapabilities));
        }
    }
}