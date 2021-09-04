using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;
using DigBuild.Engine.Worlds.Impl;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block.
    /// </summary>
    public sealed class Block
    {
        private readonly IReadOnlyDictionary<Type, GenericBlockEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IBlockAttribute, GenericBlockAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IBlockCapability, GenericBlockCapabilityDelegate> _capabilitySuppliers;
        private readonly Func<DataContainer?> _dataFactory;

        internal ISerdes<DataContainer?> DataSerdes { get; }

        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }

        internal Block(
            ResourceName name,
            IReadOnlyDictionary<Type, GenericBlockEventDelegate> eventHandlers,
            IReadOnlyDictionary<IBlockAttribute, GenericBlockAttributeDelegate> attributeSuppliers,
            IReadOnlyDictionary<IBlockCapability, GenericBlockCapabilityDelegate> capabilitySuppliers,
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
        /// Posts an event to the block.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="evt">The event</param>
        public void Post<TEvent>(TEvent evt) where TEvent : IBlockEvent
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(evt, GetDataContainer(evt));
        }

        /// <summary>
        /// Posts an event with a return value to the block.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="TOut">The return type</typeparam>
        /// <param name="evt">The event</param>
        /// <returns>The return value</returns>
        public TOut Post<TEvent, TOut>(TEvent evt) where TEvent : IBlockEvent<TOut>
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(evt, GetDataContainer(evt));
        }

        /// <summary>
        /// Gets the value of an attribute for a specific context.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="context">The context</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public TAttrib Get<TAttrib>(IReadOnlyBlockContext context, BlockAttribute<TAttrib> attribute)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered attribute: {attribute}", nameof(attribute));
            return (TAttrib) supplier(context, GetDataContainer(context));
        }
        
        /// <summary>
        /// Gets the value of a capability for a specific context.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="context">The context</param>
        /// <param name="capability">The capability</param>
        /// <returns>The value</returns>
        public TCap Get<TCap>(IBlockContext context, BlockCapability<TCap> capability)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var supplier))
                throw new ArgumentException($"Attempted to request unregistered capability: {capability}", nameof(capability));
            return (TCap) supplier(context, GetDataContainer(context));
        }

        /// <summary>
        /// Gets the value of an attribute for a specific world and position.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public TAttrib Get<TAttrib>(IReadOnlyWorld world, BlockPos pos, BlockAttribute<TAttrib> attribute)
        {
            return Get(new ReadOnlyBlockContext(world, pos, this), attribute);
        }
        
        /// <summary>
        /// Gets the value of a capability for a specific world and position.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        /// <param name="capability">The capability</param>
        /// <returns>The value</returns>
        public TCap Get<TCap>(IWorld world, BlockPos pos, BlockCapability<TCap> capability)
        {
            return Get(new BlockContext(world, pos, this), capability);
        }

        internal DataContainer? CreateDataContainer()
        {
            return _dataFactory();
        }

        private DataContainer GetDataContainer(IReadOnlyBlockContext context)
        {
            return context.World
                .GetChunk(context.Pos.ChunkPos)!
                .Get(ChunkBlocks.Type)
                .GetData(context.Pos.SubChunkPos)!;
        }

        public override string ToString()
        {
            return $"Block({Name})";
        }
    }

    /// <summary>
    /// Registry extensions for blocks.
    /// </summary>
    public static class BlockRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new block.
        /// </summary>
        /// <param name="registry">The block registry</param>
        /// <param name="domain">The domain</param>
        /// <param name="path">The path</param>
        /// <param name="buildActions">The build actions</param>
        /// <returns>The block</returns>
        public static Block Register(this IRegistryBuilder<Block> registry, string domain, string path, params Action<BlockBuilder>[] buildActions)
        {
            return registry.Register(new ResourceName(domain, path), buildActions);
        }
        
        /// <summary>
        /// Registers a new block.
        /// </summary>
        /// <param name="registry">The block registry</param>
        /// <param name="name">The name</param>
        /// <param name="buildActions">The build actions</param>
        /// <returns>The block</returns>
        public static Block Register(this IRegistryBuilder<Block> registry, ResourceName name, params Action<BlockBuilder>[] buildActions)
        {
            var builder = new BlockBuilder();
            foreach (var action in buildActions)
                action(builder);
            DigBuildEngine.EventBus.Post(new BlockBuildingEvent(name, builder));
            return registry.Add(name, builder.Build(name, DigBuildEngine.BlockEvents, DigBuildEngine.BlockAttributes, DigBuildEngine.BlockCapabilities));
        }
    }
}