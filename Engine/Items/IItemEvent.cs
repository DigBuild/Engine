using System;
using DigBuild.Engine.Reg;

namespace DigBuild.Engine.Items
{
    public interface IItemEvent
    {
    }

    public interface IItemEvent<TContext> : IItemEvent where TContext : IItemContext
    {
    }

    public interface IItemEvent<TContext, TOut> : IItemEvent where TContext : IItemContext
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
        public static void Register<TContext, TEvent>(
            this IExtendedTypeRegistryBuilder<IItemEvent, ItemEventInfo> registry,
            Action<TContext, TEvent> defaultHandler
        )
            where TContext : IItemContext
            where TEvent : IItemEvent<TContext>
        {
            registry.Add(typeof(TEvent), new ItemEventInfo((context, _, evt) =>
            {
                defaultHandler((TContext) context, (TEvent) evt);
                return null!;
            }));
        }

        public static void Register<TContext, TEvent, TResult>(
            this IExtendedTypeRegistryBuilder<IItemEvent, ItemEventInfo> registry,
            Func<TContext, TEvent, TResult> defaultHandler
        )
            where TContext : IItemContext
            where TEvent : IItemEvent<TContext, TResult>
        {
            registry.Add(typeof(TEvent), new ItemEventInfo((context, _, evt) => defaultHandler((TContext) context, (TEvent) evt)!));
        }
    }
}