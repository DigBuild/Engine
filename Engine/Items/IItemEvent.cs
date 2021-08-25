using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Items
{
    public interface IItemEvent
    {
        ItemInstance Item { get; }
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