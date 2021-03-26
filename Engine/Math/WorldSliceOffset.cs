namespace DigBuild.Engine.Math
{
    public readonly struct WorldSliceOffset
    {
        public readonly int X, Z;

        public WorldSliceOffset(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Z}>";
        }
    }
}