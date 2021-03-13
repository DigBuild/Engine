using System;
using System.Collections.Generic;
using DigBuild.Engine.Reg;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public delegate ref TOut RefFunc<in TIn, TOut>(TIn input);
    
    internal delegate object GenericBlockEventDelegate(IReadOnlyBlockContext context, BlockDataContainer dataContainer, IBlockEvent evt);
    internal delegate object GenericBlockAttributeDelegate(IReadOnlyBlockContext context, BlockDataContainer dataContainer);
    internal delegate object GenericBlockCapabilityDelegate(IBlockContext context, BlockDataContainer dataContainer);

    public sealed class BlockBuilder
    {
        private readonly List<IBlockDataHandle> _dataHandles = new();
        private readonly Dictionary<Type, List<BlockEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> _capabilitySuppliers = new();

        public BlockDataHandle<TData> Add<TData>()
            where TData : class, new()
        {
            var handle = new BlockDataHandle<TData>();
            _dataHandles.Add(handle);
            return handle;
        }

        public void Attach(IBlockBehavior<object> behavior) => AttachLast(behavior);
        public void Attach<TContract, TData>(IBlockBehavior<TContract> behavior, BlockDataHandle<TData> data)
            where TData : class, TContract, new()
            => AttachLast(behavior, data);
        public void Attach<TContract, TData>(IBlockBehavior<TContract> behavior, BlockDataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TData : class, new()
            => AttachLast(behavior, data, adapter);
        
        public void AttachLast(IBlockBehavior<object> behavior)
        {
            var builder = new BlockBehaviorBuilder<object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, false);
        }
        public void AttachLast<TContract, TData>(IBlockBehavior<TContract> behavior, BlockDataHandle<TData> data)
            where TData : class, TContract, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, false);
        }

        public void AttachLast<TContract, TData>(IBlockBehavior<TContract> behavior, BlockDataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TData : class, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, false);
        }

        public void AttachFirst(IBlockBehavior<object> behavior)
        {
            var builder = new BlockBehaviorBuilder<object>(_ => null!);
            behavior.Build(builder);
            Attach(builder, true);
        }
        public void AttachFirst<TContract, TData>(IBlockBehavior<TContract> behavior, BlockDataHandle<TData> data)
            where TData : class, TContract, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TContract>(container => container.Get(data));
            behavior.Build(builder);
            Attach(builder, true);
        }
        public void AttachFirst<TContract, TData>(IBlockBehavior<TContract> behavior, BlockDataHandle<TData> data, RefFunc<TData, TContract> adapter)
            where TData : class, new()
        {
            if (!_dataHandles.Contains(data))
                throw new ArgumentException("The specified data handle does not belong to this block.", nameof(data));

            var builder = new BlockBehaviorBuilder<TContract>(container => adapter(container.Get(data)));
            behavior.Build(builder);
            Attach(builder, true);
        }

        private void Attach(IBlockBehaviorBuilder builder, bool prepend)
        {
            foreach (var (type, list) in builder.EventHandlers)
            {
                if (!_eventHandlers.TryGetValue(type, out var blockList))
                    _eventHandlers[type] = blockList = new List<BlockEventDelegate>();
                blockList.InsertRange(prepend ? 0 : blockList.Count, list);
            }
            foreach (var (attribute, list) in builder.AttributeSuppliers)
            {
                if (!_attributeSuppliers.TryGetValue(attribute, out var blockList))
                    _attributeSuppliers[attribute] = blockList = new List<BlockAttributeDelegate>();
                blockList.InsertRange(prepend ? 0 : blockList.Count, list);
            }
            foreach (var (capability, list) in builder.CapabilitySuppliers)
            {
                if (!_capabilitySuppliers.TryGetValue(capability, out var blockList))
                    _capabilitySuppliers[capability] = blockList = new List<BlockCapabilityDelegate>();
                blockList.InsertRange(prepend ? 0 : blockList.Count, list);
            }
        }

        internal Block Build(
            ResourceName name,
            ExtendedTypeRegistry<IBlockEvent, BlockEventInfo> eventRegistry,
            Registry<IBlockAttribute> attributeRegistry,
            Registry<IBlockCapability> capabilityRegistry
        )
        {
            var eventHandlers = new Dictionary<Type, GenericBlockEventDelegate>();
            foreach (var (evtType, handlers) in _eventHandlers)
            {
                var defaultHandler = eventRegistry[evtType].DefaultHandler;
                GenericBlockEventDelegate GetDelegate(int i)
                {
                    return (context, dataContainer, evt) =>
                    {
                        if (i >= handlers.Count)
                            return defaultHandler(context, dataContainer, evt);
                        return handlers[i](context, dataContainer, evt, () => GetDelegate(i + 1)(context, dataContainer, evt));
                    };
                }
                eventHandlers[evtType] = GetDelegate(0);
            }
            foreach (var (evtType, info) in eventRegistry)
                eventHandlers.TryAdd(evtType, info.DefaultHandler);

            var attributeSuppliers = new Dictionary<IBlockAttribute, GenericBlockAttributeDelegate>();
            foreach (var (attribute, suppliers) in _attributeSuppliers)
            {
                GenericBlockAttributeDelegate GetDelegate(int i)
                {
                    return (context, dataContainer) =>
                    {
                        if (i >= suppliers.Count)
                            return attribute.GenericDefaultValueDelegate(context);
                        return suppliers[i](context, dataContainer, () => GetDelegate(i + 1)(context, dataContainer));
                    };
                }
                attributeSuppliers[attribute] = GetDelegate(0);
            }
            foreach (var attribute in attributeRegistry.Values)
                attributeSuppliers.TryAdd(attribute, (context, container) => attribute.GenericDefaultValueDelegate(context));

            var capabilitySuppliers = new Dictionary<IBlockCapability, GenericBlockCapabilityDelegate>();
            foreach (var (capability, suppliers) in _capabilitySuppliers)
            {
                GenericBlockCapabilityDelegate GetDelegate(int i)
                {
                    return (context, dataContainer) =>
                    {
                        if (i >= suppliers.Count)
                            return capability.GenericDefaultValueDelegate(context);
                        return suppliers[i](context, dataContainer, () => GetDelegate(i + 1)(context, dataContainer));
                    };
                }
                capabilitySuppliers[capability] = GetDelegate(0);
            }
            foreach (var capability in capabilityRegistry.Values)
                capabilitySuppliers.TryAdd(capability, (context, container) => capability.GenericDefaultValueDelegate(context));

            return new Block(name, eventHandlers, attributeSuppliers, capabilitySuppliers);
        }
    }
}