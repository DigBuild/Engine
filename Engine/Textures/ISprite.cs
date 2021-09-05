using System.Numerics;

namespace DigBuild.Engine.Textures
{
    /// <summary>
    /// A sprite composed of a UV coordinate and size.
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// The UV coordinates.
        /// </summary>
        public Vector2 UV { get; }
        /// <summary>
        /// The size.
        /// </summary>
        public Vector2 Size { get; }

        /// <summary>
        /// Returns an interpolated vector of the minimum and maximum UV values (based on size).
        /// </summary>
        /// <param name="partialU">The delta for the U coordinate</param>
        /// <param name="partialV">The delta for the V coordinate</param>
        /// <returns>An interpolated vector</returns>
        public Vector2 GetInterpolatedUV(float partialU, float partialV)
        {
            return UV + Size * new Vector2(partialU, partialV);
        }
    }
}