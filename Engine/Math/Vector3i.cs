using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct Vector3i
    {
        public readonly int X, Y, Z;

        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public static Vector3i operator +(Vector3i a, Vector3i b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static implicit operator Vector3i(Vector3 vec) => new((int) System.Math.Floor(vec.X), (int) System.Math.Floor(vec.Y), (int) System.Math.Floor(vec.Z));
        
        public static implicit operator Vector3(Vector3i vec) => new(vec.X, vec.Y, vec.Z);
        public static implicit operator BlockPos(Vector3i vec) => new(vec.X, vec.Y, vec.Z);
    }
}