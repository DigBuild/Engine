using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A 3D direction.
    /// </summary>
    public enum Direction : byte
    {
        NegX, PosX,
        NegY, PosY,
        NegZ, PosZ
    }

    /// <summary>
    /// A 3D axis.
    /// </summary>
    public enum Axis : byte
    {
        X, Y, Z
    }

    /// <summary>
    /// An axis direction.
    /// </summary>
    public enum AxisDirection : byte
    {
        Negative,
        Positive
    }

    /// <summary>
    /// Direction helpers.
    /// </summary>
    public static class Directions
    {
        /// <summary>
        /// A sorted set of all directions.
        /// </summary>
        public static readonly ImmutableSortedSet<Direction> All = ImmutableSortedSet.Create(
            Direction.NegX, Direction.PosX,
            Direction.NegY, Direction.PosY,
            Direction.NegZ, Direction.PosZ
        );
        /// <summary>
        /// A sorted set of all horizontal directions.
        /// </summary>
        public static readonly ImmutableSortedSet<Direction> Horizontal = ImmutableSortedSet.Create(
            Direction.NegX, Direction.PosX,
            Direction.NegZ, Direction.PosZ
        );

        /// <summary>
        /// Gets all the directions in a flag set.
        /// </summary>
        /// <param name="flags">The flags</param>
        /// <returns>The directions</returns>
        public static IEnumerable<Direction> In(DirectionFlags flags)
        {
            return All.Where(direction => flags.Has(direction));
        }
        
        /// <summary>
        /// Gets all the directions not in a flag set.
        /// </summary>
        /// <param name="flags">The flags</param>
        /// <returns>The directions</returns>
        public static IEnumerable<Direction> NotIn(DirectionFlags flags)
        {
            return All.Where(direction => !flags.Has(direction));
        }

        /// <summary>
        /// Gets a direction from a direction vector.
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <returns>The direction</returns>
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
        
        /// <summary>
        /// Gets a direction from a direction vector.
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <returns>The direction</returns>
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

    /// <summary>
    /// Direction extensions.
    /// </summary>
    public static class DirectionExtensions
    {
        private static readonly Vector3[] DirectionOffsets = {
            new(-1, 0, 0),
            new(1, 0, 0),
            new(0, -1, 0),
            new(0, 1, 0),
            new(0, 0, -1),
            new(0, 0, 1)
        };
        private static readonly Vector3I[] DirectionOffsetIs = {
            new(-1, 0, 0),
            new(1, 0, 0),
            new(0, -1, 0),
            new(0, 1, 0),
            new(0, 0, -1),
            new(0, 0, 1)
        };
        private static readonly string[] DirectionNames = {
            "neg_x",
            "pos_x",
            "neg_y",
            "pos_y",
            "neg_z",
            "pos_z"
        };
        private static readonly Vector3[] AxisVectors = {
            new(1, 0, 0),
            new(0, 1, 0),
            new(0, 0, 1)
        };
        private static readonly Vector3I[] AxisVectorIs = {
            new(1, 0, 0),
            new(0, 1, 0),
            new(0, 0, 1)
        };

        /// <summary>
        /// Deconstructs this direction into axis and axis direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <param name="axis">The axis</param>
        /// <param name="axisDirection">The axis direction</param>
        public static void Deconstruct(this Direction direction, out Axis axis, out AxisDirection axisDirection)
        {
            axis = direction.GetAxis();
            axisDirection = direction.GetAxisDirection();
        }

        /// <summary>
        /// Gets the opposite of this direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The opposite</returns>
        public static Direction GetOpposite(this Direction direction)
        {
            return (Direction) ((int) direction ^ 1);
        }

        /// <summary>
        /// Gets a vector that points in this direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The vector</returns>
        public static Vector3 GetOffset(this Direction direction)
        {
            return DirectionOffsets[(int) direction];
        }
        
        /// <summary>
        /// Gets a vector that points in this direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The vector</returns>
        public static Vector3I GetOffsetI(this Direction direction)
        {
            return DirectionOffsetIs[(int) direction];
        }

        /// <summary>
        /// Gets the name of this direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The name</returns>
        public static string GetName(this Direction direction)
        {
            return DirectionNames[(int) direction];
        }

        /// <summary>
        /// Gets the axis of this direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The axis</returns>
        public static Axis GetAxis(this Direction direction)
        {
            return (Axis) ((int) direction >> 1);
        }
        
        /// <summary>
        /// Gets the axis direction of this direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The axis direction</returns>
        public static AxisDirection GetAxisDirection(this Direction direction)
        {
            return (AxisDirection) ((int) direction & 1);
        }
        
        /// <summary>
        /// Gets a unit vector for this axis.
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <returns>The vector</returns>
        public static Vector3 AsVector(this Axis axis)
        {
            return AxisVectors[(int) axis];
        }
        
        /// <summary>
        /// Gets a unit vector for this axis.
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <returns>The vector</returns>
        public static Vector3I AsVectorI(this Axis axis)
        {
            return AxisVectorIs[(int) axis];
        }

        /// <summary>
        /// Gets the sign character for this axis direction.
        /// </summary>
        /// <param name="axisDirection">The axis direction</param>
        /// <returns>The + or - character</returns>
        public static char AsChar(this AxisDirection axisDirection)
        {
            return axisDirection == AxisDirection.Negative ? '-' : '+';
        }

        /// <summary>
        /// Converts this direction into a set of flags.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The flags</returns>
        public static DirectionFlags ToFlags(this Direction direction)
        {
            return (DirectionFlags) (1 << (int) direction);
        }
    }
}