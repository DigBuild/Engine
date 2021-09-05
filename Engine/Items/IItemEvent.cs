using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An internal item event.
    /// </summary>
    public interface IItemEvent
    {
        /// <summary>
        /// The item instance.
        /// </summary>
        ItemInstance Item { get; }
    }
    
    /// <summary>
    /// An internal item event with a return type.
    /// </summary>
    /// <typeparam name="TOut">The return type</typeparam>
    public interface IItemEvent<TOut> : IItemEvent
    {
    }
    
    /// <summary>
    /// Information about an internal item event.
    /// </summary>
    public sealed class ItemEventInfo
    {
        internal GenericItemEventDelegate DefaultHandler { get; }

        internal ItemEventInfo(GenericItemEventDelegate defaultHandler)
        {
            DefaultHandler = defaultHandler;
        }
    }
    
    /// <summary>
    /// Registry extensions for item events.
    /// </summary>
    public static class ItemEventRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new item event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="defaultHandler">The default handler</param>
        public static void Register<TEvent>(
            this ITypeRegistryBuilder<IItemEvent, ItemEventInfo> registry,
            Action<TEvent> defaultHandler
        )
            where TEvent : IItemEvent
        {
            registry.Add(typeof(TEvent), new ItemEventInfo(evt =>
            {
                defaultHandler((TEvent) evt);
                return null!;
            }));
        }
        
        /// <summary>
        /// Registers a new item event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="defaultHandler">The default handler</param>
        public static void Register<TEvent, TResult>(
            this ITypeRegistryBuilder<IItemEvent, ItemEventInfo> registry,
            Func<TEvent, TResult> defaultHandler
        )
            where TEvent : IItemEvent<TResult>
        {
            registry.Add(typeof(TEvent), new ItemEventInfo(evt => defaultHandler((TEvent) evt)!));
        }
    }
}