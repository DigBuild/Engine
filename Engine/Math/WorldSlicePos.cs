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
            return $"{nameof(WorldSlicePos)}({X}, {Z})";
        }
    }
}