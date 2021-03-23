using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public interface IItem
    {
        void Post<TContext, TEvent>(TContext context, TEvent evt)
            where TContext : IItemContext
            where TEvent : IItemEvent<TContext>;
        TOut Post<TContext, TEvent, TOut>(TContext context, TEvent evt)
            where TContext : IItemContext
            where TEvent : IItemEvent<TContext, TOut>;
    }

    public sealed class Item : IItem
    {
        private readonly IReadOnlyDictionary<Type, GenericItemEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IItemAttribute, GenericItemAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IItemCapability, GenericItemCapabilityDelegate> _capabilitySuppliers;

        public ResourceName Name { get; }

        internal Item(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericItemEventDelegate> eventHandlers,
            IReadOnlyDictionary<IItemAttribute, GenericItemAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IItemCapability, GenericItemCapabilityDelegate> capabilitySuppliers
        )
        {
            _eventHandlers = eventHandlers;
            _attributeSuppliers = attributeSuppliers;
            _capabilitySuppliers = capabilitySuppliers;
            Name = name;
        }

        void IItem.Post<TContext, TEvent>(TContext context, TEvent evt)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(context, GetDataContainer(context), evt);
        }

        TOut IItem.Post<TContext, TEvent, TOut>(TContext context, TEvent evt)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(context, GetDataContainer(context), evt);
        }

        public TAttrib Get<TAttrib>(IItemContext context, ItemAttribute<TAttrib> attribute)
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
        
        private ItemDataContainer GetDataContainer(IItemContext context)
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