using System;
using System.Numerics;

namespace DigBuild.Engine.Math
{
    public readonly struct BlockPos
    {
        public readonly int X, Y, Z;

        public ChunkPos ChunkPos => new(X >> 4, Y >> 4, Z >> 4);

        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public BlockPos(Vector3 vec)
        {
            X = (int) System.MathF.Floor(vec.X);
            Y = (int) System.MathF.Floor(vec.Y);
            Z = (int) System.MathF.Floor(vec.Z);
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
    }
}