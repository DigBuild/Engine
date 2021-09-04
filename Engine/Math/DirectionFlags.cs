using System;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A set of direction flags.
    /// </summary>
    [Flags]
    public enum DirectionFlags : byte
    {
        None = 0,
        NegX = 1 << 0,
        PosX = 1 << 1,
        NegY = 1 << 2,
        PosY = 1 << 3,
        NegZ = 1 << 4,
        PosZ = 1 << 5,
        All = NegX | PosX | NegY | PosY | NegZ | PosZ
    }

    /// <summary>
    /// Direction flag extensions.
    /// </summary>
    public static class BlockFaceFlagsExtensions
    {
        /// <summary>
        /// Checks whether a flag set contains a direction or not.
        /// </summary>
        /// <param name="flags">The flags</param>
        /// <param name="direction">The direction</param>
        /// <returns>Whether it is contained or not</returns>
        public static bool Has(this DirectionFlags flags, Direction direction)
        {
            return (flags & direction.ToFlags()) != DirectionFlags.None;
        }
    }
}