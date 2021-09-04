using DigBuild.Engine.Blocks;
using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.BuiltIn
{
    /// <summary>
    /// A built-in block event.
    /// </summary>
    public abstract class BuiltInBlockEvent : BlockContext, IBlockEvent
    {
        private BuiltInBlockEvent(IWorld world, BlockPos pos, Block block) : base(world, pos, block)
        {
        }

        /// <summary>
        /// Fired when the block is added to the world.
        /// </summary>
        public class JoinedWorld : BuiltInBlockEvent
        {
            public JoinedWorld(IWorld world, BlockPos pos, Block block) : base(world, pos, block)
            {
            }
        }

        /// <summary>
        /// Fired when the block is being removed from the world.
        /// </summary>
        public class LeavingWorld : BuiltInBlockEvent
        {
            public LeavingWorld(IWorld world, BlockPos pos, Block block) : base(world, pos, block)
            {
            }
        }
    }

    /// <summary>
    /// Registration/subscription extensions for built-in block events.
    /// </summary>
    public static class BuiltInBlockEventExtensions
    {
        /// <summary>
        /// Subscribes to the joined world event.
        /// </summary>
        /// <typeparam name="TReadOnlyData">The read-only data type</typeparam>
        /// <typeparam name="TData">The read-write data type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="onJoinedWorld">The handler</param>
        public static void Subscribe<TReadOnlyData, TData>(
            this IBlockBehaviorBuilder<TReadOnlyData, TData> builder,
            BlockEventDelegate<TData, BuiltInBlockEvent.JoinedWorld> onJoinedWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onJoinedWorld);
        }

        /// <summary>
        /// Fires the joined world event.
        /// </summary>
        /// <param name="block">The block</param>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        public static void OnJoinedWorld(this Block block, IWorld world, BlockPos pos)
        {
            block.Post(new BuiltInBlockEvent.JoinedWorld(world, pos, block));
        }
        
        /// <summary>
        /// Subscribes to the leaving world event.
        /// </summary>
        /// <typeparam name="TReadOnlyData">The read-only data type</typeparam>
        /// <typeparam name="TData">The read-write data type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="onLeavingWorld">The handler</param>
        public static void Subscribe<TReadOnlyData, TData>(
            this IBlockBehaviorBuilder<TReadOnlyData, TData> builder,
            BlockEventDelegate<TData, BuiltInBlockEvent.LeavingWorld> onLeavingWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onLeavingWorld);
        }
        
        /// <summary>
        /// Fires the leaving world event.
        /// </summary>
        /// <param name="block">The block</param>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        public static void OnLeavingWorld(this Block block, IWorld world, BlockPos pos)
        {
            block.Post(new BuiltInBlockEvent.LeavingWorld(world, pos, block));
        }
    }
}