using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct ChunkPos : IEquatable<ChunkPos>
    {
        public int X { get; }
        public int Z { get; }

        public RegionPos RegionPos => new(X >> 6, Z >> 6);
        public RegionChunkPos RegionChunkPos => new(X & 63, Z & 63);

        public ChunkPos(int x, int z)
        {
            X = x;
            Z = z;
        }
        
        public void Deconstruct(out RegionPos regionPos, out RegionChunkPos chunkPos)
        {
            regionPos = RegionPos;
            chunkPos = RegionChunkPos;
        }

        public Vector3 GetCenter(float y)
        {
            return new Vector3((X << 4) + 8, y, (Z << 4) + 8);
        }

        public Vector3 GetOrigin()
        {
            return new Vector3(X << 4, 0, Z << 4);
        }

        public long DistanceSq(ChunkPos other)
        {
            var x = X - other.X;
            var z = Z - other.Z;
            return x * x + z * z;
        }

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
            (chunkPos.X << 4) | subChunkPos.X,
            subChunkPos.Y,
            (chunkPos.Z << 4) | subChunkPos.Z
        );

        public static ChunkPos operator +(ChunkPos pos, ChunkOffset offset) => new(pos.X + offset.X, pos.Z + offset.Z);
        public static ChunkOffset operator -(ChunkPos a, ChunkPos b) => new(a.X - b.X, a.Z - b.Z);
    }
}