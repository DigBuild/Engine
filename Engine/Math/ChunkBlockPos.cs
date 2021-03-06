﻿using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct ChunkBlockPos : IEquatable<ChunkBlockPos>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        
        public ChunkBlockPos(int x, int y, int z)
        {
            X = x & 15;
            Y = y & 255;
            Z = z & 15;
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