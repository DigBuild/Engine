using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block context.
    /// </summary>
    public interface IBlockContext : IReadOnlyBlockContext
    {
        /// <summary>
        /// The world.
        /// </summary>
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