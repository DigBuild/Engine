using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct Vector3I : IVector3I, IEquatable<Vector3I>
    {
        public static Vector3I Zero = new(0, 0, 0);
        public static Vector3I One = new(1, 1, 1);

        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3I(IVector3I vec) : this(vec.X, vec.Y, vec.Z)
        {
        }

        public Vector3I(Vector3 vec) : this(
            (int) System.Math.Floor(vec.X),
            (int) System.Math.Floor(vec.Y),
            (int) System.Math.Floor(vec.Z)
        )
        {
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        
        public Vector3I Offset(Direction face, int amount = 1)
        {
            return this + face.GetOffsetI() * amount;
        }

        public long LengthSquared()
        {
            long x = X, y = Y, z = Z;
            return x * x + y * y + z * z;
        }

        public double Length()
        {
            return System.Math.Sqrt(LengthSquared());
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public bool Equals(IVector3I? other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public bool Equals(Vector3I other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is IVector3I other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Vector3I left, IVector3I right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3I left, IVector3I right)
        {
            return !left.Equals(right);
        }
        
        public static Vector3I operator -(Vector3I vec) => new(-vec.X, -vec.Y, -vec.Z);
        public static Vector3I operator +(Vector3I a, IVector3I b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3I operator -(Vector3I a, IVector3I b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3I operator *(Vector3I vec, int amount) => new(vec.X * amount, vec.Y * amount, vec.Z * amount);
        
        public static explicit operator Vector3(Vector3I vec) => new(vec.X, vec.Y, vec.Z);
        public static implicit operator Vector3I((int X, int Y, int Z) tuple) => new(tuple.X, tuple.Y, tuple.Z);

        public static Vector3I Abs(IVector3I vector)
        {
            return new(System.Math.Abs(vector.X), System.Math.Abs(vector.Y), System.Math.Abs(vector.Z));
        }

        public static Vector3I Min(IVector3I a, IVector3I b)
        {
            return new(
                System.Math.Min(a.X, b.X),
                System.Math.Min(a.Y, b.Y),
                System.Math.Min(a.Z, b.Z)
            );
        }

        public static Vector3I Max(IVector3I a, IVector3I b)
        {
            return new(
                System.Math.Max(a.X, b.X),
                System.Math.Max(a.Y, b.Y),
                System.Math.Max(a.Z, b.Z)
            );
        }
    }
}