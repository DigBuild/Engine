namespace DigBuild.Engine.Math
{
    public readonly struct SubChunkPos
    {
        public readonly int X, Y, Z;
        
        public SubChunkPos(int x, int y, int z)
        {
            X = x & 15;
            Y = y & 15;
            Z = z & 15;
        }
        
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static implicit operator SubChunkPos(BlockPos pos) => new(pos.X, pos.Y, pos.Z);
    }
}