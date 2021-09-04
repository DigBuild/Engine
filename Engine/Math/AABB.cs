using System;
using System.Collections.Generic;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// An axis-aligned bounding box.
    /// </summary>
    public readonly struct AABB
    {
        private const float Epsilon = 1e-7f;

        /// <summary>
        /// A full block (1x1x1 anchored at the origin) bounding box.
        /// </summary>
        public static readonly AABB FullBlock = new(Vector3.Zero, Vector3.One);

        /// <summary>
        /// The lowermost corner.
        /// </summary>
        public Vector3 Min { get; }
        /// <summary>
        /// The highermost corner.
        /// </summary>
        public Vector3 Max { get; }

        /// <summary>
        /// The center.
        /// </summary>
        public Vector3 Center => (Min + Max) / 2;

        public AABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public AABB(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);
        }

        /// <summary>
        /// Creates a new AABB that is expanded by the specified amount in each direction.
        /// </summary>
        /// <param name="x">The X amount</param>
        /// <param name="y">The Y amount</param>
        /// <param name="z">The Z amount</param>
        /// <returns>A new AABB</returns>
        public AABB Grow(float x, float y, float z)
        {
            var amt = new Vector3(x, y, z);
            return new AABB(Min - amt, Max + amt);
        }

        /// <summary>
        /// Creates a new AABB that is expanded by the specified amount in all directions.
        /// </summary>
        /// <param name="amt">The amount</param>
        /// <returns>A new AABB</returns>
        public AABB Grow(float amt) => Grow(amt, amt, amt);

        /// <summary>
        /// Checks whether the point is contained within this AABB or not.
        /// </summary>
        /// <param name="vec">The point</param>
        /// <returns>Whether it is contained or not</returns>
        public bool Contains(Vector3 vec)
        {
            return
                vec.X >= Min.X && vec.Y >= Min.Y && vec.Z >= Min.Z &&
                vec.X < Max.X && vec.Y < Max.Y && vec.Z < Max.Z;
        }
        
        /// <summary>
        /// Checks whether the point is contained within this AABB or not and returns how far in it is.
        /// </summary>
        /// <param name="vec">The point</param>
        /// <param name="penetration">The penetration</param>
        /// <returns>Whether it is contained or not</returns>
        public bool Contains(Vector3 vec, out Vector3 penetration)
        {
            penetration = Vector3.Zero;

            var a = vec - Min;
            var b = Max - vec;
            if (a.X < 0 || a.Y < 0 || a.Z < 0 || b.X <= 0 || b.Y <= 0 || b.Z <= 0)
                return false;

            var amt = float.MaxValue;
            if (a.X < amt)
            {
                amt = a.X;
                penetration = new Vector3(a.X, 0, 0);
            }
            if (a.Y < amt)
            {
                amt = a.Y;
                penetration = new Vector3(0, a.Y, 0);
            }
            if (a.Z < amt)
            {
                amt = a.Z;
                penetration = new Vector3(0, 0, a.Z);
            }
            if (b.X < amt)
            {
                amt = b.X;
                penetration = new Vector3(-b.X, 0, 0);
            }
            if (b.Y < amt)
            {
                amt = b.Y;
                penetration = new Vector3(0, -b.Y, 0);
            }
            if (b.Z < amt)
            {
                penetration = new Vector3(0, 0, -b.Z);
            }
            return true;
        }

        /// <summary>
        /// Checks whether another AABB intersects this one or not.
        /// </summary>
        /// <param name="other">The other AABB</param>
        /// <returns>Whether they intersect or not</returns>
        public bool Intersects(AABB other)
        {
            return
                Min.X + Epsilon < other.Max.X && Max.X > other.Min.X + Epsilon &&
                Min.Y + Epsilon < other.Max.Y && Max.Y > other.Min.Y + Epsilon &&
                Min.Z + Epsilon < other.Max.Z && Max.Z > other.Min.Z + Epsilon;
        }
        
        /// <summary>
        /// Checks whether another AABB intersects this one or not and returns how far in it is.
        /// </summary>
        /// <param name="other">The other AABB</param>
        /// <param name="intersection">The intersection</param>
        /// <returns>Whether they intersect or not</returns>
        public bool Intersects(AABB other, out Vector3 intersection)
        {
            float x1 = other.Max.X - Min.X, x2 = Max.X - other.Min.X;
            float y1 = other.Max.Y - Min.Y, y2 = Max.Y - other.Min.Y;
            float z1 = other.Max.Z - Min.Z, z2 = Max.Z - other.Min.Z;
            intersection = new Vector3(
                x1 < x2 ? x1 : -x2,
                y1 < y2 ? y1 : -y2,
                z1 < z2 ? z1 : -z2
            );
            return !(x1 < Epsilon || x2 < Epsilon || y1 < Epsilon || y2 < Epsilon || z1 < Epsilon || z2 < Epsilon);
        }
        
        /// <summary>
        /// Tries to intersect a ray and this AABB.
        /// </summary>
        /// <param name="origin">The ray origin</param>
        /// <param name="magnitude">The ray magnitude</param>
        /// <param name="delta">How far the ray travelled</param>
        /// <param name="side">The side it hit</param>
        /// <returns>Whether the ray intersects the AABB or not</returns>
        public bool IntersectRay(Vector3 origin, Vector3 magnitude, out float delta, out Direction side)
        {
            delta = float.MaxValue;
            side = Direction.NegX;

            var min = Min - origin;
            var max = Max - origin;

            if (min.X > 0 && magnitude.X >= min.X)
            {
                var d = min.X / magnitude.X;
                var hit = magnitude * d;
                if (hit.Y >= min.Y && hit.Y < max.Y && hit.Z >= min.Z && hit.Z < max.Z && d < delta)
                {
                    delta = d;
                    side = Direction.NegX;
                }
            }
            if (max.X < 0 && magnitude.X <= max.X)
            {
                var d = max.X / magnitude.X;
                var hit = magnitude * d;
                if (hit.Y >= min.Y && hit.Y < max.Y && hit.Z >= min.Z && hit.Z < max.Z && d < delta)
                {
                    delta = d;
                    side = Direction.PosX;
                }
            }

            if (min.Y > 0 && magnitude.Y >= min.Y)
            {
                var d = min.Y / magnitude.Y;
                var hit = magnitude * d;
                if (hit.X >= min.X && hit.X < max.X && hit.Z >= min.Z && hit.Z < max.Z && d < delta)
                {
                    delta = d;
                    side = Direction.NegY;
                }
            }
            if (max.Y < 0 && magnitude.Y <= max.Y)
            {
                var d = max.Y / magnitude.Y;
                var hit = magnitude * d;
                if (hit.X >= min.X && hit.X < max.X && hit.Z >= min.Z && hit.Z < max.Z && d < delta)
                {
                    delta = d;
                    side = Direction.PosY;
                }
            }

            if (min.Z > 0 && magnitude.Z >= min.Z)
            {
                var d = min.Z / magnitude.Z;
                var hit = magnitude * d;
                if (hit.X >= min.X && hit.X < max.X && hit.Y >= min.Y && hit.Y < max.Y && d < delta)
                {
                    delta = d;
                    side = Direction.NegZ;
                }
            }
            if (max.Z < 0 && magnitude.Z <= max.Z)
            {
                var d = max.Z / magnitude.Z;
                var hit = magnitude * d;
                if (hit.X >= min.X && hit.X < max.X && hit.Y >= min.Y && hit.Y < max.Y && d < delta)
                {
                    delta = d;
                    side = Direction.PosZ;
                }
            }

            return delta != float.MaxValue;
        }

        /// <summary>
        /// Gets all the block positions intersected by this AABB.
        /// </summary>
        /// <returns>The enumerable of block positions</returns>
        public IEnumerable<BlockPos> GetIntersectedBlockPositions()
        {
            var (minX, minY, minZ) = new BlockPos(Min);
            var (maxX, maxY, maxZ) = new BlockPos(Max);
            for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
            for (var z = minZ; z <= maxZ; z++)
                yield return new BlockPos(x, y, z);
        }

        public override string ToString()
        {
            return $"{nameof(AABB)}({Min}, {Max})";
        }
        
        public static AABB operator +(AABB aabb, Vector3 offset) => new(aabb.Min + offset, aabb.Max + offset);
        public static AABB operator -(AABB aabb, Vector3 offset) => new(aabb.Min - offset, aabb.Max - offset);

        /// <summary>
        /// Creates a new AABB containing both arguments.
        /// </summary>
        /// <param name="a">The first AABB</param>
        /// <param name="b">The second AABB</param>
        /// <returns>The new AABB</returns>
        public static AABB Containing(AABB a, AABB b)
        {
            return new AABB(
                MathF.Min(a.Min.X, b.Min.X),
                MathF.Min(a.Min.Y, b.Min.Y),
                MathF.Min(a.Min.Z, b.Min.Z),
                MathF.Max(a.Max.X, b.Max.X),
                MathF.Max(a.Max.Y, b.Max.Y),
                MathF.Max(a.Max.Z, b.Max.Z)
            );
        }

        /// <summary>
        /// Calculates the Minkowski difference of two AABBs.
        /// </summary>
        /// <param name="a">The first AABB</param>
        /// <param name="b">The second AABB</param>
        /// <returns>The new AABB</returns>
        public static AABB MinkowskiDifference(AABB a, AABB b)
        {
            return new AABB(
                a.Min - b.Max,
                a.Max - b.Min
            );
        }
    }
}