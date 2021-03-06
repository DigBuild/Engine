using System.Collections.Immutable;
using System.Numerics;
using DigBuild.Engine.Render;

namespace DigBuild.Engine.Math
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

        public static BlockFace FromOffset(Vector3 vector)
        {
            var abs = Vector3.Abs(vector);
            if (abs.X > abs.Y)
            {
                if (abs.X > abs.Z)
                    return vector.X > 0 ? BlockFace.PosX : BlockFace.NegX;
                return vector.Z > 0 ? BlockFace.PosZ : BlockFace.NegZ;
            }
            else
            {
                if (abs.Y > abs.Z)
                    return vector.Y > 0 ? BlockFace.PosY : BlockFace.NegY;
                return vector.Z > 0 ? BlockFace.PosZ : BlockFace.NegZ;
            }
        }
    }

    public static class BlockFaceExtensions
    {
        public static BlockFaceFlags ToFlags(this BlockFace face)
        {
            return (BlockFaceFlags) (1 << (int) face);
        }
    }
}