using System;
using System.Collections.Generic;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct AABB
    {
        private const float Epsilon = 1e-7f;

        public static readonly AABB FullBlock = new(0, 0, 0, 1, 1, 1);

        public readonly Vector3 Min, Max;

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

        public bool Contains(Vector3 vec)
        {
            return
                vec.X >= Min.X && vec.Y >= Min.Y && vec.Z >= Min.Z &&
                vec.X < Max.X && vec.Y < Max.Y && vec.Z < Max.Z;
        }

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

        public bool Intersects(AABB other)
        {
            return
                Min.X + Epsilon < other.Max.X && Max.X > other.Min.X + Epsilon &&
                Min.Y + Epsilon < other.Max.Y && Max.Y > other.Min.Y + Epsilon &&
                Min.Z + Epsilon < other.Max.Z && Max.Z > other.Min.Z + Epsilon;
        }

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

        public static AABB Containing(AABB a, AABB b)
        {
            return new(
                MathF.Min(a.Min.X, b.Min.X),
                MathF.Min(a.Min.Y, b.Min.Y),
                MathF.Min(a.Min.Z, b.Min.Z),
                MathF.Max(a.Max.X, b.Max.X),
                MathF.Max(a.Max.Y, b.Max.Y),
                MathF.Max(a.Max.Z, b.Max.Z)
            );
        }

        public static AABB MinkowskiDifference(AABB a, AABB b)
        {
            return new(
                a.Min - b.Max,
                a.Max - b.Min
            );
        }
    }
}