using DigBuild.Engine.Math;

namespace DigBuild.Engine.Blocks
{
    public readonly struct BlockFaceSolidity
    {
        public static BlockAttribute<BlockFaceSolidity> Attribute { get; internal set; } = null!;
        public static BlockFaceSolidity None { get; } = new(DirectionFlags.None);
        public static BlockFaceSolidity All { get; } = new(DirectionFlags.All);

        public DirectionFlags Solid { get; }

        public BlockFaceSolidity(DirectionFlags solid)
        {
            Solid = solid;
        }
    }
}