using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block context.
    /// </summary>
    public class BlockContext : IBlockContext
    {
        public IWorld World { get; }
        public BlockPos Pos { get; }
        public Block Block { get; }

        public BlockContext(IBlockContext context) : this(context.World, context.Pos, context.Block)
        {
        }

        public BlockContext(IWorld world, BlockPos pos, Block block)
        {
            World = world;
            Pos = pos;
            Block = block;
        }
    }
}