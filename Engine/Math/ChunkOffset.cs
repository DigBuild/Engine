using System;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A chunk offset.
    /// </summary>
    public readonly struct ChunkOffset
    {
        /// <summary>
        /// A zero chunk offset.
        /// </summary>
        public static ChunkOffset Zero { get; } = new();
        /// <summary>
        /// A unit chunk offset.
        /// </summary>
        public static ChunkOffset One { get; } = new(1, 1);

        /// <summary>
        /// The X offset.
        /// </summary>
        public int X { get; }
        /// <summary>
        /// The Z offset.
        /// </summary>
        public int Z { get; }

        public ChunkOffset(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Deconstructs the chunk offset into its components.
        /// </summary>
        /// <param name="x">The X offset</param>
        /// <param name="z">The Z offset</param>
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