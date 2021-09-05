using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A chunk position.
    /// </summary>
    public readonly struct ChunkPos : IEquatable<ChunkPos>
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X { get; }
        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public int Z { get; }

        /// <summary>
        /// The region position.
        /// </summary>
        public RegionPos RegionPos => new(
            WorldDimensions.ChunkCoordToRegionCoord(X),
            WorldDimensions.ChunkCoordToRegionCoord(Z)
        );
        /// <summary>
        /// The position within the region.
        /// </summary>
        public RegionChunkPos RegionChunkPos => new(X, Z);

        public ChunkPos(int x, int z)
        {
            X = x;
            Z = z;
        }
        
        /// <summary>
        /// Deconstructs the chunk into a region and region-local position.
        /// </summary>
        /// <param name="regionPos">The region position</param>
        /// <param name="chunkPos">The position within the region</param>
        public void Deconstruct(out RegionPos regionPos, out RegionChunkPos chunkPos)
        {
            regionPos = RegionPos;
            chunkPos = RegionChunkPos;
        }

        /// <summary>
        /// Gets the center of the chunk.
        /// </summary>
        /// <param name="y">The Y coordinate</param>
        /// <returns>The center position</returns>
        public Vector3 GetCenter(float y)
        {
            return new Vector3(
                WorldDimensions.XZChunkAndSubChunkCoordToBlockCoord(X, (int)WorldDimensions.ChunkSize / 2),
                y,
                WorldDimensions.XZChunkAndSubChunkCoordToBlockCoord(Z, (int)WorldDimensions.ChunkSize / 2)
            );
        }

        /// <summary>
        /// Gets the lowermost corner of the chunk.
        /// </summary>
        /// <returns>The origin position</returns>
        public Vector3 GetOrigin()
        {
            return new Vector3(
                WorldDimensions.XZChunkCoordToBlockCoord(X), 
                0,
                WorldDimensions.XZChunkCoordToBlockCoord(Z)
            );
        }

        /// <summary>
        /// Calculates the square distance to another chunk.
        /// </summary>
        /// <param name="other">The other position</param>
        /// <returns>The square distance</returns>
        public long DistanceSq(ChunkPos other)
        {
            var x = X - other.X;
            var z = Z - other.Z;
            return x * x + z * z;
        }

        /// <summary>
        /// Creates a chunk position offset in the specified direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The new position</returns>
        public ChunkPos Offset(Direction direction)
        {
            var offset = direction.GetOffsetI();
            return new ChunkPos(
                X + offset.X,
                Z + offset.Z
            );
        }

        public override string ToString()
        {
            return $"<{X}, {Z}>";
        }
        
        public bool Equals(ChunkPos other)
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

        public static bool operator ==(ChunkPos left, ChunkPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkPos left, ChunkPos right)
        {
            return !left.Equals(right);
        }

        public static BlockPos operator +(ChunkPos chunkPos, ChunkBlockPos subChunkPos) => new(
            WorldDimensions.XZChunkAndSubChunkCoordToBlockCoord(chunkPos.X, subChunkPos.X),
            subChunkPos.Y,
            WorldDimensions.XZChunkAndSubChunkCoordToBlockCoord(chunkPos.Z, subChunkPos.Z)
        );

        public static ChunkPos operator +(ChunkPos pos, ChunkOffset offset) => new(pos.X + offset.X, pos.Z + offset.Z);
        public static ChunkOffset operator -(ChunkPos a, ChunkPos b) => new(a.X - b.X, a.Z - b.Z);
    }
}