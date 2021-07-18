using System;
using System.Collections.Generic;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public sealed class Block
    {
        private readonly IReadOnlyDictionary<Type, GenericBlockEventDelegate> _eventHandlers;
        private readonly IReadOnlyDictionary<IBlockAttribute, GenericBlockAttributeDelegate> _attributeSuppliers;
        private readonly IReadOnlyDictionary<IBlockCapability, GenericBlockCapabilityDelegate> _capabilitySuppliers;
        private readonly Func<DataContainer?> _dataFactory;

        internal ISerdes<DataContainer?> DataSerdes { get; }

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

        public void Post<TEvent>(TEvent evt) where TEvent : IBlockEvent
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            handler(evt, GetDataContainer(evt));
        }

        public TOut Post<TEvent, TOut>(TEvent evt) where TEvent : IBlockEvent<TOut>
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handler))
                throw new ArgumentException($"Attempted to post unregistered event: {typeof(TEvent)}", nameof(evt));
            return (TOut) handler(evt, GetDataContainer(evt));
        }

        public TAttrib Get<TAttrib>(IReadOnlyBlockContext context, BlockAttribute<TAttrib> attribute)
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

    public static class BlockRegistryBuilderExtensions
    {
        public static Block Create(this IRegistryBuilder<Block> registry, string domain, string path, params Action<BlockBuilder>[] buildActions)
        {
            return registry.Create(new ResourceName(domain, path), buildActions);
        }

        public static Block Create(this IRegistryBuilder<Block> registry, ResourceName name, params Action<BlockBuilder>[] buildActions)
        {
            var builder = new BlockBuilder();
            foreach (var action in buildActions)
                action(builder);
            return registry.Add(name, builder.Build(name, BuiltInRegistries.BlockEvents, BuiltInRegistries.BlockAttributes, BuiltInRegistries.BlockCapabilities));
        }
    }
}