using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A read-only block context.
    /// </summary>
    public interface IReadOnlyBlockContext
    {
        /// <summary>
        /// The world.
        /// </summary>
        public IReadOnlyWorld World { get; }
        /// <summary>
        /// The position.
        /// </summary>
        public BlockPos Pos { get; }
        /// <summary>
        /// The block.
        /// </summary>
        public Block Block { get; }

        public void Deconstruct(out IReadOnlyWorld world, out BlockPos pos, out Block block)
        {
            world = World;
            pos = Pos;
            block = Block;
        }
    }
}