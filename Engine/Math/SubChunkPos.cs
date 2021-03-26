using System;

namespace DigBuild.Engine.Math
{
    public readonly struct SubChunkPos : IEquatable<SubChunkPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        
        public SubChunkPos(int x, int y, int z)
        {
            X = x & 15;
            Y = y & 15;
            Z = z & 15;
        }
        
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
        
        public bool Equals(SubChunkPos other)
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

        public static bool operator ==(SubChunkPos left, SubChunkPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SubChunkPos left, SubChunkPos right)
        {
            return !left.Equals(right);
        }
    }
}