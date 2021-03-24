using DigBuild.Engine.Blocks;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInBlockEvent
    {
        public class JoinedWorld : IBlockEvent<IBlockContext>
        {
        }

        public class LeavingWorld : IBlockEvent<IBlockContext>
        {
        }
    }

    public static class BuiltInBlockEventExtensions
    {
        public static void Subscribe<TReadOnlyData, TData>(
            this IBlockBehaviorBuilder<TReadOnlyData, TData> builder,
            BlockEventDelegate<IBlockContext, TData, BuiltInBlockEvent.JoinedWorld> onJoinedWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onJoinedWorld);
        }

        public static void OnJoinedWorld(this IBlock block, IBlockContext context, BuiltInBlockEvent.JoinedWorld evt)
        {
            block.Post(context, evt);
        }

        public static void Subscribe<TReadOnlyData, TData>(
            this IBlockBehaviorBuilder<TReadOnlyData, TData> builder,
            BlockEventDelegate<IBlockContext, TData, BuiltInBlockEvent.LeavingWorld> onLeavingWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onLeavingWorld);
        }

        public static void OnLeavingWorld(this IBlock block, IBlockContext context, BuiltInBlockEvent.LeavingWorld evt)
        {
            block.Post(context, evt);
        }
    }
}