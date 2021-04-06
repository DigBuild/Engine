using DigBuild.Engine.Blocks;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInBlockEvent
    {
        public class JoinedWorld : IBlockEvent
        {
        }

        public class LeavingWorld : IBlockEvent
        {
        }
    }

    public static class BuiltInBlockEventExtensions
    {
        public static void Subscribe<TReadOnlyData, TData>(
            this IBlockBehaviorBuilder<TReadOnlyData, TData> builder,
            BlockEventDelegate<TData, BuiltInBlockEvent.JoinedWorld> onJoinedWorld
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
            BlockEventDelegate<TData, BuiltInBlockEvent.LeavingWorld> onLeavingWorld
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