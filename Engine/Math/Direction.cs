using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using DigBuild.Engine.Render;

namespace DigBuild.Engine.Math
{
    public enum Direction : byte
    {
        NegX, PosX,
        NegY, PosY,
        NegZ, PosZ
    }

    public static class Directions
    {
        public static readonly ImmutableSortedSet<Direction> All = ImmutableSortedSet.Create(
            Direction.NegX, Direction.PosX,
            Direction.NegY, Direction.PosY,
            Direction.NegZ, Direction.PosZ
        );

        public static IEnumerable<Direction> In(DirectionFlags flags)
        {
            return All.Where(direction => flags.Has(direction));
        }

        public static Direction FromOffset(Vector3 vector)
        {
            var abs = Vector3.Abs(vector);
            if (abs.X > abs.Y)
            {
                if (abs.X > abs.Z)
                    return vector.X > 0 ? Direction.PosX : Direction.NegX;
                return vector.Z > 0 ? Direction.PosZ : Direction.NegZ;
            }
            else
            {
                if (abs.Y > abs.Z)
                    return vector.Y > 0 ? Direction.PosY : Direction.NegY;
                return vector.Z > 0 ? Direction.PosZ : Direction.NegZ;
            }
        }

        public static Direction FromOffset(IVector3I vector)
        {
            var abs = Vector3I.Abs(vector);
            if (abs.X > abs.Y)
            {
                if (abs.X > abs.Z)
                    return vector.X > 0 ? Direction.PosX : Direction.NegX;
                return vector.Z > 0 ? Direction.PosZ : Direction.NegZ;
            }
            else
            {
                if (abs.Y > abs.Z)
                    return vector.Y > 0 ? Direction.PosY : Direction.NegY;
                return vector.Z > 0 ? Direction.PosZ : Direction.NegZ;
            }
        }
    }

    public static class DirectionExtensions
    {
        private static readonly Vector3I[] Offsets = {
            new(-1, 0, 0),
            new(1, 0, 0),
            new(0, -1, 0),
            new(0, 1, 0),
            new(0, 0, -1),
            new(0, 0, 1)
        };

        public static Direction GetOpposite(this Direction direction)
        {
            return (Direction) ((int) direction ^ 1);
        }

        public static Vector3I GetOffset(this Direction direction)
        {
            return Offsets[(int) direction];
        }

        public static DirectionFlags ToFlags(this Direction direction)
        {
            return (DirectionFlags) (1 << (int) direction);
        }
    }
}