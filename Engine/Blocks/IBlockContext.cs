using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

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
}