using System;
using System.Collections.Generic;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Blocks
{
    public delegate void BlockEventDelegate<in TData, in TEvent>(TEvent evt, TData data, Action next)
        where TEvent : IBlockEvent;
    public delegate TResult BlockEventDelegate<in TData, in TEvent, TResult>(TEvent evt, TData data, Func<TResult> next)
        where TEvent : IBlockEvent<TResult>;
    
    public delegate T BlockAttributeDelegate<in TData, T>(IReadOnlyBlockContext context, TData data, Func<T> next);
    public delegate T BlockCapabilityDelegate<in TData, T>(IBlockContext context, TData data, Func<T> next);
    
    internal delegate object BlockEventDelegate(IBlockEvent evt, DataContainer? dataContainer, Func<object> next);
    internal delegate object BlockAttributeDelegate(IReadOnlyBlockContext context, DataContainer? dataContainer, Func<object> next);
    internal delegate object BlockCapabilityDelegate(IBlockContext context, DataContainer? dataContainer, Func<object> next);

    public interface IBlockBehaviorBuilder
    {
        internal Dictionary<Type, List<BlockEventDelegate>> EventHandlers { get; }
        internal Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> AttributeSuppliers { get; }
        internal Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> CapabilitySuppliers { get; }
    }

    public interface IBlockBehaviorBuilder<out TReadOnlyData, out TData> : IBlockBehaviorBuilder
        where TData : TReadOnlyData
    {
        void Subscribe<TEvent>(BlockEventDelegate<TData, TEvent> del)
            where TEvent : IBlockEvent;
        void Subscribe<TEvent, TResult>(BlockEventDelegate<TData, TEvent, TResult> del)
            where TEvent : IBlockEvent<TResult>;
        
        void Add<T>(BlockAttribute<T> attribute, BlockAttributeDelegate<TReadOnlyData, T> supplier);
        void Add<T>(BlockCapability<T> capability, BlockCapabilityDelegate<TData, T> supplier);
    }

    public sealed class BlockBehaviorBuilder<TReadOnlyData, TData> : IBlockBehaviorBuilder<TReadOnlyData, TData>
        where TData : TReadOnlyData
    {
        private readonly Dictionary<Type, List<BlockEventDelegate>> _eventHandlers = new();
        private readonly Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> _attributeSuppliers = new();
        private readonly Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> _capabilitySuppliers = new();

        Dictionary<Type, List<BlockEventDelegate>> IBlockBehaviorBuilder.EventHandlers => _eventHandlers;
        Dictionary<IBlockAttribute, List<BlockAttributeDelegate>> IBlockBehaviorBuilder.AttributeSuppliers => _attributeSuppliers;
        Dictionary<IBlockCapability, List<BlockCapabilityDelegate>> IBlockBehaviorBuilder.CapabilitySuppliers => _capabilitySuppliers;

        private readonly Func<DataContainer?, TData> _dataGetter;

        internal BlockBehaviorBuilder(Func<DataContainer?, TData> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        void IBlockBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent>(BlockEventDelegate<TData, TEvent> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<BlockEventDelegate>();
            list.Add((evt, dataContainer, next) =>
            {
                del((TEvent) evt, _dataGetter(dataContainer), () => next());
                return null!;
            });
        }

        void IBlockBehaviorBuilder<TReadOnlyData, TData>.Subscribe<TEvent, TResult>(BlockEventDelegate<TData, TEvent, TResult> del)
        {
            var type = typeof(TEvent);
            if (!_eventHandlers.TryGetValue(type, out var list))
                _eventHandlers[type] = list = new List<BlockEventDelegate>();
            list.Add((evt, dataContainer, next) =>
            {
                return del((TEvent) evt, _dataGetter(dataContainer), () => (TResult) next())!;
            });
        }

        public void Add<T>(BlockAttribute<T> attribute, BlockAttributeDelegate<TReadOnlyData, T> supplier)
        {
            if (!_attributeSuppliers.TryGetValue(attribute, out var list))
                _attributeSuppliers[attribute] = list = new List<BlockAttributeDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), () => (T) next())!;
            });
        }

        public void Add<T>(BlockCapability<T> capability, BlockCapabilityDelegate<TData, T> supplier)
        {
            if (!_capabilitySuppliers.TryGetValue(capability, out var list))
                _capabilitySuppliers[capability] = list = new List<BlockCapabilityDelegate>();
            list.Add((context, dataContainer, next) =>
            {
                return supplier(context, _dataGetter(dataContainer), () => (T) next())!;
            });
        }
    }
}