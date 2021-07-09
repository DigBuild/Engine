using System;

namespace DigBuild.Engine.Math
{
    public readonly struct RegionChunkPos : IEquatable<RegionChunkPos>
    {
        public int X { get; }
        public int Z { get; }

        public RegionChunkPos(int x, int z)
        {
            X = x & 63;
            Z = z & 63;
        }

        public override string ToString()
        {
            return $"<{X}, {Z}>";
        }
        
        public bool Equals(RegionChunkPos other)
        {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is ChunkPos other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
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