using System;

namespace DigBuild.Engine.Math
{
    public readonly struct RegionChunkPos : IEquatable<RegionChunkPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public RegionChunkPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
        
        public bool Equals(RegionChunkPos other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is ChunkPos other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(RegionChunkPos left, RegionChunkPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RegionChunkPos left, RegionChunkPos right)
        {
            return !left.Equals(right);
        }
    }
}