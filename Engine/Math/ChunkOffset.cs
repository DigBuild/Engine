using System;

namespace DigBuild.Engine.Math
{
    public readonly struct ChunkOffset
    {
        public static ChunkOffset Zero { get; } = new();
        public static ChunkOffset One { get; } = new(1, 1);

        public readonly int X, Z;

        public ChunkOffset(int x, int z)
        {
            X = x;
            Z = z;
        }

        public void Deconstruct(out int x, out int z)
        {
            x = X;
            z = Z;
        }

        public override string ToString()
        {
            return $"<{X}, {Z}>";
        }

        public static ChunkOffset operator +(ChunkOffset a, ChunkOffset b) => new(a.X + b.X, a.Z + b.Z);
        public static ChunkOffset operator -(ChunkOffset a, ChunkOffset b) => new(a.X - b.X, a.Z - b.Z);
    }
}