using System;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A chunk position within a region.
    /// </summary>
    public readonly struct RegionChunkPos : IEquatable<RegionChunkPos>
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X { get; }
        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public int Z { get; }

        public RegionChunkPos(int x, int z)
        {
            X = WorldDimensions.ChunkCoordToSubRegionCoord(x);
            Z = WorldDimensions.ChunkCoordToSubRegionCoord(z);
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