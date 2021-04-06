using System;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Blocks
{
    public interface IBlockEvent
    {
    }

    public interface IBlockEvent<TOut> : IBlockEvent
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
        public static void Register<TEvent>(
            this IExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry,
            Action<IBlockContext, TEvent> defaultHandler
        )
            where TEvent : IBlockEvent
        {
            registry.Add(typeof(TEvent), new BlockEventInfo((context, _, evt) =>
            {
                defaultHandler(context, (TEvent) evt);
                return null!;
            }));
        }

        public static void Register<TEvent, TResult>(
            this IExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry,
            Func<IBlockContext, TEvent, TResult> defaultHandler
        )
            where TEvent : IBlockEvent<TResult>
        {
            registry.Add(typeof(TEvent), new BlockEventInfo((context, _, evt) => defaultHandler(context, (TEvent) evt)!));
        }
    }
}