using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct ChunkPos : IEquatable<ChunkPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public ChunkPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 GetCenter()
        {
            return new((X << 4) + 8, (Y << 4) + 8, (Z << 4) + 8);
        }

        public Vector3 GetOrigin()
        {
            return new(X << 4, Y << 4, Z << 4);
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
        
        public bool Equals(ChunkPos other)
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

        public static bool operator ==(ChunkPos left, ChunkPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkPos left, ChunkPos right)
        {
            return !left.Equals(right);
        }

        public static BlockPos operator +(ChunkPos chunkPos, SubChunkPos subChunkPos) => new(
            (chunkPos.X << 4) | subChunkPos.X,
            (chunkPos.Y << 4) | subChunkPos.Y,
            (chunkPos.Z << 4) | subChunkPos.Z
        );
    }
}