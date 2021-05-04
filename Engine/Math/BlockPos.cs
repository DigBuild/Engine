using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct BlockPos : IVector3I, IEquatable<BlockPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        
        public ChunkPos ChunkPos => new(X >> 4, Y >> 4, Z >> 4);
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

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        
        public void Deconstruct(out ChunkPos chunkPos, out ChunkBlockPos subChunkPos)
        {
            chunkPos = ChunkPos;
            subChunkPos = SubChunkPos;
        }

        public BlockPos Offset(Direction face, int amount = 1)
        {
            return this + face.GetOffsetI() * amount;
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