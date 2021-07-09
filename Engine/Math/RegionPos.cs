using System;

namespace DigBuild.Engine.Math
{
    public readonly struct RegionPos : IEquatable<RegionPos>
    {
        public int X { get; }
        public int Z { get; }

        public RegionPos(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Z}>";
        }
        
        public bool Equals(RegionPos other)
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

        public static bool operator ==(RegionPos left, RegionPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RegionPos left, RegionPos right)
        {
            return !left.Equals(right);
        }
        
        public static ChunkPos operator +(RegionPos regionPos, RegionChunkPos chunkPos) => new(
            (regionPos.X << 6) | chunkPos.X,
            (regionPos.Z << 6) | chunkPos.Z
        );
    }
}