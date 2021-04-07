using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInBlockEvent
    {
        public class JoinedWorld : BlockContext, IBlockEvent
        {
            public JoinedWorld(IWorld world, BlockPos pos, Block block) : base(world, pos, block)
            {
            }
        }

        public class LeavingWorld : BlockContext, IBlockEvent
        {
            public LeavingWorld(IWorld world, BlockPos pos, Block block) : base(world, pos, block)
            {
            }
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

        public static void OnJoinedWorld(this Block block, IWorld world, BlockPos pos)
        {
            block.Post(new BuiltInBlockEvent.JoinedWorld(world, pos, block));
        }

        public static void Subscribe<TReadOnlyData, TData>(
            this IBlockBehaviorBuilder<TReadOnlyData, TData> builder,
            BlockEventDelegate<TData, BuiltInBlockEvent.LeavingWorld> onLeavingWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onLeavingWorld);
        }

        public static void OnLeavingWorld(this Block block, IWorld world, BlockPos pos)
        {
            block.Post(new BuiltInBlockEvent.LeavingWorld(world, pos, block));
        }
    }
}