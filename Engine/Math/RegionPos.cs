using System;

namespace DigBuild.Engine.Math
{
    public readonly struct RegionPos : IEquatable<RegionPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public RegionPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
        
        public bool Equals(RegionPos other)
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
            (regionPos.Y << 6) | chunkPos.Y,
            (regionPos.Z << 6) | chunkPos.Z
        );
    }
}