using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuildEngine.Math;

namespace DigBuildEngine.Math
{
    public readonly struct AABB
    {
        private const float Epsilon = 1e-7f;

        public static readonly AABB FullBlock = new(0, 0, 0, 1, 1, 1);

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

        public AABB WithOffset(Vector3 offset)
        {
            return new(Min + offset, Max + offset);
        }

        public bool Contains(Vector3 vec)
        {
            return
                vec.X >= Min.X && vec.Y >= Min.Y && vec.Z >= Min.Z &&
                vec.X < Max.X && vec.Y < Max.Y && vec.Z < Max.Z;
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
            if (x1 < Epsilon || x2 < Epsilon || y1 < Epsilon || y2 < Epsilon || z1 < Epsilon || z2 < Epsilon)
            {
                intersection = Vector3.Zero;
                return false;
            }
            intersection = new Vector3(
                x1 < x2 ? x1 : -x2,
                y1 < y2 ? y1 : -y2,
                z1 < z2 ? z1 : -z2
            );
            return true;
        }

        public IEnumerable<BlockPos> GetIntersectedBlockPositions()
        {
            int minX = (int) MathF.Floor(Min.X), maxX = (int) MathF.Ceiling(Max.X);
            int minY = (int) MathF.Floor(Min.Y), maxY = (int) MathF.Ceiling(Max.Y);
            int minZ = (int) MathF.Floor(Min.Z), maxZ = (int) MathF.Ceiling(Max.Z);
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    for (var z = minZ; z <= maxZ; z++)
                    {
                        yield return new BlockPos(x, y, z);
                    }
                }
            }
        }
    }
}