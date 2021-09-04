using System;
using DigBuild.Engine.Events;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// An internal block event.
    /// </summary>
    public interface IBlockEvent : IEvent, IBlockContext
    {
    }
    
    /// <summary>
    /// An internal block event with a return type.
    /// </summary>
    /// <typeparam name="TOut">The return type</typeparam>
    public interface IBlockEvent<TOut> : IBlockEvent
    {
    }

    /// <summary>
    /// Information about an internal block event.
    /// </summary>
    public sealed class BlockEventInfo
    {
        internal GenericBlockEventDelegate DefaultHandler { get; }

        internal BlockEventInfo(GenericBlockEventDelegate defaultHandler)
        {
            DefaultHandler = defaultHandler;
        }
    }
    
    /// <summary>
    /// Registry extensions for block events.
    /// </summary>
    public static class BlockEventRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new block event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="defaultHandler">The default handler</param>
        public static void Register<TEvent>(
            this ITypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry,
            Action<TEvent> defaultHandler
        )
            where TEvent : IBlockEvent
        {
            registry.Add(typeof(TEvent), new BlockEventInfo((evt, _) =>
            {
                defaultHandler((TEvent) evt);
                return null!;
            }));
        }

        /// <summary>
        /// Registers a new block event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="defaultHandler">The default handler</param>
        public static void Register<TEvent, TResult>(
            this ITypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry,
            Func<TEvent, TResult> defaultHandler
        )
            where TEvent : IBlockEvent<TResult>
        {
            registry.Add(typeof(TEvent), new BlockEventInfo((evt, _) => defaultHandler((TEvent) evt)!));
        }
    }
}