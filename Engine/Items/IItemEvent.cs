using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Items
{
    public interface IItemEvent
    {
    }
    
    public interface IItemEvent<TOut> : IItemEvent
    {
    }

    public sealed class ItemEventInfo
    {
        internal GenericItemEventDelegate DefaultHandler { get; }

        internal ItemEventInfo(GenericItemEventDelegate defaultHandler)
        {
            DefaultHandler = defaultHandler;
        }
    }
    
    public static class ItemEventRegistryBuilderExtensions
    {
        public static void Register<TEvent>(
            this IExtendedTypeRegistryBuilder<IItemEvent, ItemEventInfo> registry,
            Action<IItemContext, TEvent> defaultHandler
        )
            where TEvent : IItemEvent
        {
            registry.Add(typeof(TEvent), new ItemEventInfo((context, _, evt) =>
            {
                defaultHandler(context, (TEvent) evt);
                return null!;
            }));
        }

        public static void Register<TEvent, TResult>(
            this IExtendedTypeRegistryBuilder<IItemEvent, ItemEventInfo> registry,
            Func<IItemContext, TEvent, TResult> defaultHandler
        )
            where TEvent : IItemEvent<TResult>
        {
            registry.Add(typeof(TEvent), new ItemEventInfo((context, _, evt) => defaultHandler(context, (TEvent) evt)!));
        }
    }
}