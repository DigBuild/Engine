using DigBuild.Engine.Math;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Blocks
{
    public interface IBlockContext : IReadOnlyBlockContext
    {
        public new IWorld World { get; }
        
        IReadOnlyWorld IReadOnlyBlockContext.World => World;

        public void Deconstruct(out IWorld world, out BlockPos pos, out Block block)
        {
            world = World;
            pos = Pos;
            block = Block;
        }
    }

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