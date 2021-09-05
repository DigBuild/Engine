using System;
using DigBuild.Engine.Events;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An internal entity event.
    /// </summary>
    public interface IEntityEvent : IEvent
    {
        EntityInstance Entity { get; }
    }
    
    /// <summary>
    /// An internal entity event with a return type.
    /// </summary>
    /// <typeparam name="TOut">The return type</typeparam>
    public interface IEntityEvent<TOut> : IEntityEvent
    {
    }
    
    /// <summary>
    /// Information about an internal entity event.
    /// </summary>
    public sealed class EntityEventInfo
    {
        internal GenericEntityEventDelegate DefaultHandler { get; }

        internal EntityEventInfo(GenericEntityEventDelegate defaultHandler)
        {
            DefaultHandler = defaultHandler;
        }
    }
    
    /// <summary>
    /// Registry extensions for entity events.
    /// </summary>
    public static class EntityEventRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new entity event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="defaultHandler">The default handler</param>
        public static void Register<TEvent>(
            this ITypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry,
            Action<TEvent> defaultHandler
        )
            where TEvent : IEntityEvent
        {
            registry.Add(typeof(TEvent), new EntityEventInfo(evt =>
            {
                defaultHandler((TEvent) evt);
                return null!;
            }));
        }
        
        /// <summary>
        /// Registers a new entity event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="defaultHandler">The default handler</param>
        public static void Register<TEvent, TResult>(
            this ITypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry,
            Func<TEvent, TResult> defaultHandler
        )
            where TEvent : IEntityEvent<TResult>
        {
            registry.Add(typeof(TEvent), new EntityEventInfo(evt => defaultHandler((TEvent) evt)!));
        }
    }
}