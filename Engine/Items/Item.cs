using System;
using System.Collections.Generic;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An item.
    /// </summary>
    public sealed class Item
    {
        private readonly IReadOnlyDictionary<Type, GenericItemEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IItemAttribute, GenericItemAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IItemCapability, GenericItemCapabilityDelegate> _capabilitySuppliers;
        private readonly Func<DataContainer?> _dataFactory;
        private readonly Func<DataContainer?, DataContainer?, bool> _equalityTest;

        internal ISerdes<DataContainer?> DataSerdes { get; }
        
        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }

        internal Item(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericItemEventDelegate> eventHandlers,
            IReadOnlyDictionary<IItemAttribute, GenericItemAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IItemCapability, GenericItemCapabilityDelegate> capabilitySuppliers,
            Func<DataContainer?> dataFactory,
            ISerdes<DataContainer?> dataSerdes,
            Func<DataContainer?, DataContainer?, bool> equalityTest
        )
        {
            _eventHandlers = eventHandlers;
            _attributeSuppliers = attributeSuppliers;
            _capabilitySuppliers = capabilitySuppliers;
            _dataFactory = dataFactory;
            _equalityTest = equalityTest;
            DataSerdes = dataSerdes;
            Name = name;
        }
        
        /// <summary>
        /// Posts an event to the item.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="evt">The event</param>
        public void Post<TEvent>(TEvent evt) where TEvent : IItemEvent
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(evt);
        }
        
        /// <summary>
        /// Posts an event with a return value to the item.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="TOut">The return type</typeparam>
        /// <param name="evt">The event</param>
        /// <returns>The return value</returns>
        public TOut Post<TEvent, TOut>(TEvent evt) where TEvent : IItemEvent<TOut>
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(evt);
        }
        
        /// <summary>
        /// Gets the value of an attribute for a specific item instance.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public TAttrib Get<TAttrib>(IReadOnlyItemInstance instance, ItemAttribute<TAttrib> attribute)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered attribute: {attribute}", nameof(attribute));
            return (TAttrib) supplier(instance);
        }
        
        /// <summary>
        /// Gets the value of a capability for a specific item instance.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="capability">The capability</param>
        /// <returns>The value</returns>
        public TCap Get<TCap>(ItemInstance instance, ItemCapability<TCap> capability)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered capability: {capability}", nameof(capability));
            return (TCap) supplier(instance);
        }

        internal DataContainer? CreateDataContainer()
        {
            return _dataFactory();
        }

        internal bool Equals(DataContainer? first, DataContainer? second)
        {
            return _equalityTest(first, second);
        }

        public override string ToString()
        {
            return $"Item({Name})";
        }
    }
    
    /// <summary>
    /// Registry extensions for items.
    /// </summary>
    public static class ItemRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new item.
        /// </summary>
        /// <param name="registry">The item registry</param>
        /// <param name="name">The name</param>
        /// <param name="buildActions">The build actions</param>
        /// <returns>The block</returns>
        public static Item Register(this IRegistryBuilder<Item> registry, ResourceName name, params Action<ItemBuilder>[] buildActions)
        {
            var builder = new ItemBuilder();
            foreach (var action in buildActions)
                action(builder);
            DigBuildEngine.EventBus.Post(new ItemBuildingEvent(name, builder));
            return registry.Add(name, builder.Build(name, DigBuildEngine.ItemEvents, DigBuildEngine.ItemAttributes, DigBuildEngine.ItemCapabilities));
        }
    }
}