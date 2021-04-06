using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Entities
{
    public interface IEntityEvent
    {
    }

    public interface IEntityEvent<TOut> : IEntityEvent
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
        public static void Register<TEvent>(
            this IExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry,
            Action<IEntityContext, TEvent> defaultHandler
        )
            where TEvent : IEntityEvent
        {
            registry.Add(typeof(TEvent), new EntityEventInfo((context, _, evt) =>
            {
                defaultHandler(context, (TEvent) evt);
                return null!;
            }));
        }

        public static void Register<TEvent, TResult>(
            this IExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry,
            Func<IEntityContext, TEvent, TResult> defaultHandler
        )
            where TEvent : IEntityEvent<TResult>
        {
            registry.Add(typeof(TEvent), new EntityEventInfo((context, _, evt) => defaultHandler(context, (TEvent) evt)!));
        }
    }
}