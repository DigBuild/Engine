using DigBuild.Engine.Math;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Blocks
{
    public interface IReadOnlyBlockContext
    {
        public IReadOnlyWorld World { get; }
        public BlockPos Pos { get; }
        public Block Block { get; }

        public void Deconstruct(out IReadOnlyWorld world, out BlockPos pos, out Block block)
        {
            world = World;
            pos = Pos;
            block = Block;
        }
    }
}