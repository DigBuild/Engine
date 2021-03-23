using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Entities
{
    public interface IEntityEvent
    {
    }

    public interface IEntityEvent<TContext> : IEntityEvent where TContext : IEntityContext
    {
    }

    public interface IEntityEvent<TContext, TOut> : IEntityEvent where TContext : IEntityContext
    {
    }

    public sealed class EntityEventInfo
    {
        internal GenericEntityEventDelegate DefaultHandler { get; }

        internal EntityEventInfo(GenericEntityEventDelegate defaultHandler)
        {
            DefaultHandler = defaultHandler;
        }
    }
    
    public static class EntityEventRegistryBuilderExtensions
    {
        public static void Register<TContext, TEvent>(
            this IExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry,
            Action<TContext, TEvent> defaultHandler
        )
            where TContext : IEntityContext
            where TEvent : IEntityEvent<TContext>
        {
            registry.Add(typeof(TEvent), new EntityEventInfo((context, _, evt) =>
            {
                defaultHandler((TContext) context, (TEvent) evt);
                return null!;
            }));
        }

        public static void Register<TContext, TEvent, TResult>(
            this IExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry,
            Func<TContext, TEvent, TResult> defaultHandler
        )
            where TContext : IEntityContext
            where TEvent : IEntityEvent<TContext, TResult>
        {
            registry.Add(typeof(TEvent), new EntityEventInfo((context, _, evt) => defaultHandler((TContext) context, (TEvent) evt)!));
        }
    }
}