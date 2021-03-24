using System;
using System.Collections.Generic;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Blocks
{
    public delegate void BlockEventDelegate<in TContext, in TData, in TEvent>(TContext context, TData data, TEvent evt, Action next)
        where TContext : IBlockContext
        where TEvent : IBlockEvent<TContext>;
    public delegate TResult BlockEventDelegate<in TContext, in TData, in TEvent, TResult>(TContext context, TData data, TEvent evt, Func<TResult> next)
        where TContext : IBlockContext
        where TEvent : IBlockEvent<TContext, TResult>;
    
    public delegate T BlockAttributeDelegate<in TData, T>(IReadOnlyBlockContext context, TData data, BlockAttribute<T> attribute, Func<T> next);
    public delegate T BlockCapabilityDelegate<in TData, T>(IBlockContext context, TData data, BlockCapability<T> capability, Func<T> next);
    
    internal delegate object BlockEventDelegate(IBlockContext context, DataContainer dataContainer, IBlockEvent evt, Func<object> next);
    internal delegate object BlockAttributeDelegate(IReadOnlyBlockContext context, DataContainer dataContainer, Func<object> next);
    internal delegate object BlockCapabilityDelegate(IBlockContext context, DataContainer dataContainer, Func<object> next);

    public interface IBlockBehaviorBuilder
    {
        internal Dictionary<Type, List<BlockEventDelegate>> EventHandlers { get; }
        internal Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> AttributeSuppliers { get; }
        internal Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> CapabilitySuppliers { get; }
    }

    public interface IBlockBehaviorBuilder<out TData> : IBlockBehaviorBuilder
    {
        void Subscribe<TContext, TEvent>(BlockEventDelegate<TContext, TData, TEvent> del)
            where TContext : IBlockContext
            where TEvent : IBlockEvent<TContext>;
        void Subscribe<TContext, TEvent, TResult>(BlockEventDelegate<TContext, TData, TEvent, TResult> del)
            where TContext : IBlockContext
            where TEvent : IBlockEvent<TContext, TResult>;
        
        void Add<T>(BlockAttribute<T> attribute, BlockAttributeDelegate<TData, T> supplier);
        void Add<T>(BlockCapability<T> capability, BlockCapabilityDelegate<TData, T> supplier);
    }

    public sealed class BlockBehaviorBuilder<TData> : IBlockBehaviorBuilder<TData>
    {
        private readonly Dictionary<Type, List<BlockEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> _capabilitySuppliers = new();

        Dictionary<Type, List<BlockEventDelegate>> IBlockBehaviorBuilder.EventHandlers => _eventHandlers;
        Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> IBlockBehaviorBuilder.AttributeSuppliers => _attributeSuppliers;
        Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> IBlockBehaviorBuilder.CapabilitySuppliers => _capabilitySuppliers;

        private readonly Func<DataContainer, TData> _dataGetter;

        internal BlockBehaviorBuilder(Func<DataContainer, TData> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        void IBlockBehaviorBuilder<TData>.Subscribe<TContext, TEvent>(BlockEventDelegate<TContext, TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<BlockEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                del((TContext) context, _dataGetter(dataContainer), (TEvent) evt, () => next());
                return null!;
            });
        }

        void IBlockBehaviorBuilder<TData>.Subscribe<TContext, TEvent, TResult>(BlockEventDelegate<TContext, TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<BlockEventDelegate>();
            list.Add((context, dataContainer, evt, next) =>
            {
                return del((TContext) context, _dataGetter(dataContainer), (TEvent) evt, () => (TResult) next())!;
            });
        }

        public void Add<T>(BlockAttribute<T> attribute, BlockAttributeDelegate<TData, T> supplier)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var list))
                _attributeSuppliers[attribute] = list = new List<BlockAttributeDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), attribute, () => (T) next())!;
            });
        }

        public void Add<T>(BlockCapability<T> capability, BlockCapabilityDelegate<TData, T> supplier)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var list))
                _capabilitySuppliers[capability] = list = new List<BlockCapabilityDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), capability, () => (T) next())!;
            });
        }
    }
}