using System.Collections.Immutable;
using DigBuildEngine.Render;

namespace DigBuildEngine.Math
{
    public enum BlockFace : byte
    {
        NegX, PosX,
        NegY, PosY,
        NegZ, PosZ
    }

    public static class BlockFaces
    {
        public static readonly ImmutableSortedSet<BlockFace> All = ImmutableSortedSet.Create(
            BlockFace.NegX, BlockFace.PosX,
            BlockFace.NegY, BlockFace.PosY,
            BlockFace.NegZ, BlockFace.PosZ
        );
    }

    public static class BlockFaceExtensions
    {
        public static BlockFaceFlags ToFlags(this BlockFace face)
        {
            return (BlockFaceFlags) (1 << (int) face);
        }
    }
}