using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Blocks
{
    public sealed class BlockContext : IBlockContext
    {
        public IWorld World { get; }
        public BlockPos Pos { get; }
        public Block Block { get; }

        public BlockContext(IWorld world, BlockPos pos, Block block)
        {
            World = world;
            Pos = pos;
            Block = block;
        }
    }
}