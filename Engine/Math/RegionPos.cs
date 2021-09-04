using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A region position.
    /// </summary>
    public readonly struct RegionPos : IEquatable<RegionPos>
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X { get; }
        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public int Z { get; }

        public RegionPos(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Gets the lowermost corner of the chunk.
        /// </summary>
        /// <returns>The origin position</returns>
        public Vector3 GetOrigin()
        {
            return new Vector3(
                WorldDimensions.XZChunkCoordToBlockCoord(WorldDimensions.RegionCoordToChunkCoord(X)), 
                0,
                WorldDimensions.XZChunkCoordToBlockCoord(WorldDimensions.RegionCoordToChunkCoord(Z))
            );
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
            return obj is RegionPos other && Equals(other);
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
            WorldDimensions.RegionAndSubRegionCoordToChunkCoord(regionPos.X, chunkPos.X),
            WorldDimensions.RegionAndSubRegionCoordToChunkCoord(regionPos.Z, chunkPos.Z)
        );
    }
}