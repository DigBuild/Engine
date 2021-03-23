using System;
using System.Collections.Generic;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public interface IBlock
    {
        void Post<TContext, TEvent>(TContext context, TEvent evt)
            where TContext : IReadOnlyBlockContext
            where TEvent : IBlockEvent<TContext>;
        TOut Post<TContext, TEvent, TOut>(TContext context, TEvent evt)
            where TContext : IReadOnlyBlockContext
            where TEvent : IBlockEvent<TContext, TOut>;
    }

    public sealed class Block : IBlock
    {
        private readonly IReadOnlyDictionary<Type, GenericBlockEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IBlockAttribute, GenericBlockAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IBlockCapability, GenericBlockCapabilityDelegate> _capabilitySuppliers;
        private readonly Action<IBlockContext, BlockDataContainer> _dataInitializer;

        public ResourceName Name { get; }

        internal Block(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericBlockEventDelegate> eventHandlers,
            IReadOnlyDictionary<IBlockAttribute, GenericBlockAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IBlockCapability, GenericBlockCapabilityDelegate> capabilitySuppliers,
            Action<IBlockContext, BlockDataContainer> dataInitializer
        )
        {
            _eventHandlers = eventHandlers;
            _attributeSuppliers = attributeSuppliers;
            _capabilitySuppliers = capabilitySuppliers;
            _dataInitializer = dataInitializer;
            Name = name;
        }

        void IBlock.Post<TContext, TEvent>(TContext context, TEvent evt)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(context, GetDataContainer(context), evt);
        }

        TOut IBlock.Post<TContext, TEvent, TOut>(TContext context, TEvent evt)
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(context, GetDataContainer(context), evt);
        }

        public TAttrib Get<TAttrib>(IBlockContext context, BlockAttribute<TAttrib> attribute)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered attribute: {attribute}", nameof(attribute));
            return (TAttrib) supplier(context, GetDataContainer(context));
        }

        public TCap Get<TCap>(IBlockContext context, BlockCapability<TCap> capability)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered capability: {capability}", nameof(capability));
            return (TCap) supplier(context, GetDataContainer(context));
        }

        internal BlockDataContainer CreateDataContainer(IBlockContext context)
        {
            var container = new BlockDataContainer();
            _dataInitializer(context, container);
            return container;
        }

        private BlockDataContainer GetDataContainer(IReadOnlyBlockContext context)
        {
            return ((IWorld)context.World).GetData(context.Pos)!;
        }

        public override string ToString()
        {
            return $"Block({Name})";
        }
    }

    public static class BlockRegistryBuilderExtensions
    {
        public static ExtendedTypeRegistry<IBlockEvent, BlockEventInfo> EventRegistry = null!;
        public static Registry<IBlockAttribute> BlockAttributes = null!;
        public static Registry<IBlockCapability> BlockCapabilities = null!;

        public static Block Create(this IRegistryBuilder<Block> registry, ResourceName name, params Action<BlockBuilder>[] buildActions)
        {
            var builder = new BlockBuilder();
            foreach (var action in buildActions)
                action(builder);
            return registry.Add(name, builder.Build(name, EventRegistry, BlockAttributes, BlockCapabilities));
        }
    }
}