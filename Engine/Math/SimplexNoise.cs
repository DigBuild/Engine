namespace DigBuild.Engine.Math
{
    /// <summary>
    /// Simplex noise generator based on FastNoiseLite.
    /// </summary>
    public sealed class SimplexNoise
    {
        private readonly long _random;
        private readonly FastNoiseLite _noise = new();

        /// <summary>
        /// The seed.
        /// </summary>
        public long Seed
        {
            set => _noise.SetSeed((int) ((1342143 * (_random % 1235980234)) ^ value + _random - value));
        }

        public SimplexNoise(long random, float frequency, uint octaves, float lacunarity = 2.0f, float gain = 0.5f)
        {
            _random = random;
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise.SetFrequency(frequency);
            _noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            _noise.SetFractalOctaves((int) octaves);
            _noise.SetFractalLacunarity(lacunarity);
            _noise.SetFractalGain(gain);
        }

        /// <summary>
        /// The noise at a specific position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>The noise value between -1 and 1</returns>
        public float this[long x, long y] => _noise.GetNoise(x, y);
    }
}