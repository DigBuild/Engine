namespace DigBuild.Engine.Math
{
    public readonly struct WorldSlicePos
    {
        public readonly int X, Z;

        public WorldSlicePos(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Z}>";
        }

        public static WorldSlicePos operator +(WorldSlicePos pos, WorldSliceOffset offset)
        {
            return new(pos.X + offset.X, pos.Z + offset.Z);
        }
    }
}