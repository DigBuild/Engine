using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A block position.
    /// </summary>
    public readonly struct BlockPos : IVector3I, IEquatable<BlockPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        
        /// <summary>
        /// The block's chunk position.
        /// </summary>
        public ChunkPos ChunkPos => new(
            WorldDimensions.XZBlockCoordToChunkCoord(X),
            WorldDimensions.XZBlockCoordToChunkCoord(Z)
        );
        /// <summary>
        /// The block's position within the chunk.
        /// </summary>
        public ChunkBlockPos SubChunkPos => new(X, Y, Z);

        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public BlockPos(IVector3I vec) : this(vec.X, vec.Y, vec.Z)
        {
        }

        public BlockPos(Vector3 vec) : this(
            (int) System.Math.Floor(vec.X),
            (int) System.Math.Floor(vec.Y),
            (int) System.Math.Floor(vec.Z)
        )
        {
        }

        /// <summary>
        /// Deconstructs the block position into its components.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        
        /// <summary>
        /// Deconstructs the block position into its chunk and chunk-relative positions.
        /// </summary>
        /// <param name="chunkPos">The chunk position</param>
        /// <param name="subChunkPos">The position within the chunk</param>
        public void Deconstruct(out ChunkPos chunkPos, out ChunkBlockPos subChunkPos)
        {
            chunkPos = ChunkPos;
            subChunkPos = SubChunkPos;
        }

        /// <summary>
        /// Creates a new block position offset by the specified amount in the given direction.
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <param name="amount">The amount</param>
        /// <returns>A new block position</returns>
        public BlockPos Offset(Direction direction, int amount = 1)
        {
            return this + direction.GetOffsetI() * amount;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public bool Equals(IVector3I? other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public bool Equals(BlockPos other)
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

        public static bool operator ==(BlockPos left, IVector3I right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockPos left, IVector3I right)
        {
            return !left.Equals(right);
        }
        
        public static BlockPos operator +(BlockPos a, IVector3I b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static BlockPos operator -(BlockPos a, IVector3I b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        
        public static explicit operator BlockPos(Vector3I vec) => new(vec.X, vec.Y, vec.Z);
        public static explicit operator Vector3(BlockPos pos) => new(pos.X, pos.Y, pos.Z);
    }
}