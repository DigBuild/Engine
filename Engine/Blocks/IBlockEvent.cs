using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Blocks
{
    public interface IBlockEvent
    {
    }

    public interface IBlockEvent<TContext> : IBlockEvent where TContext : IReadOnlyBlockContext
    {
    }

    public interface IBlockEvent<TContext, TOut> : IBlockEvent where TContext : IReadOnlyBlockContext
    {
    }

    public sealed class BlockEventInfo
    {
        internal GenericBlockEventDelegate DefaultHandler { get; }

        internal BlockEventInfo(GenericBlockEventDelegate defaultHandler)
        {
            DefaultHandler = defaultHandler;
        }
    }
    
    public static class BlockEventRegistryBuilderExtensions
    {
        public static void Register<TContext, TEvent>(
            this IExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry,
            Action<TContext, TEvent> defaultHandler
        )
            where TContext : IReadOnlyBlockContext
            where TEvent : IBlockEvent<TContext>
        {
            registry.Add(typeof(TEvent), new BlockEventInfo((context, _, evt) =>
            {
                defaultHandler((TContext) context, (TEvent) evt);
                return null!;
            }));
        }

        public static void Register<TContext, TEvent, TResult>(
            this IExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry,
            Func<TContext, TEvent, TResult> defaultHandler
        )
            where TContext : IReadOnlyBlockContext
            where TEvent : IBlockEvent<TContext, TResult>
        {
            registry.Add(typeof(TEvent), new BlockEventInfo((context, _, evt) => defaultHandler((TContext) context, (TEvent) evt)!));
        }
    }
}