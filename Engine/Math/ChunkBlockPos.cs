﻿using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A block position within a chunk.
    /// </summary>
    public readonly struct ChunkBlockPos : IEquatable<ChunkBlockPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        
        public ChunkBlockPos(int x, int y, int z)
        {
            X = WorldDimensions.XZBlockCoordToSubChunkCoord(x);
            Y = (int)(y % WorldDimensions.ChunkHeight);
            Z = WorldDimensions.XZBlockCoordToSubChunkCoord(z);
        }
        
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
        
        public bool Equals(ChunkBlockPos other)
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

        public static bool operator ==(ChunkBlockPos left, ChunkBlockPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkBlockPos left, ChunkBlockPos right)
        {
            return !left.Equals(right);
        }

        public static explicit operator Vector3(ChunkBlockPos pos) => new(pos.X, pos.Y, pos.Z);
    }
}