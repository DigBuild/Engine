using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        public static IEnumerable<BlockFace> In(BlockFaceFlags flags)
        {
            return All.Where(face => flags.Has(face));
        }

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
        private static readonly Vector3i[] Offsets = {
            new(-1, 0, 0),
            new(1, 0, 0),
            new(0, -1, 0),
            new(0, 1, 0),
            new(0, 0, -1),
            new(0, 0, 1)
        };

        public static BlockFace GetOpposite(this BlockFace face)
        {
            return (BlockFace) ((int) face ^ 1);
        }

        public static Vector3i GetOffset(this BlockFace face)
        {
            return Offsets[(int) face];
        }

        public static BlockFaceFlags ToFlags(this BlockFace face)
        {
            return (BlockFaceFlags) (1 << (int) face);
        }
    }
}