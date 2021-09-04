using DigBuild.Engine.Math;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block's face solidity.
    /// </summary>
    public readonly struct BlockFaceSolidity
    {
        /// <summary>
        /// The attribute.
        /// </summary>
        public static BlockAttribute<BlockFaceSolidity> Attribute { get; internal set; } = null!;
        /// <summary>
        /// No faces are solid.
        /// </summary>
        public static BlockFaceSolidity None { get; } = new(DirectionFlags.None);
        /// <summary>
        /// All faces are solid.
        /// </summary>
        public static BlockFaceSolidity All { get; } = new(DirectionFlags.All);

        /// <summary>
        /// The faces that are solid.
        /// </summary>
        public DirectionFlags Solid { get; }

        public BlockFaceSolidity(DirectionFlags solid)
        {
            Solid = solid;
        }
    }
}