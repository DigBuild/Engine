using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    /// <summary>
    /// A grid-aligned ray casting context.
    /// </summary>
    /// <typeparam name="THit">The returned hit type</typeparam>
    public interface IGridAlignedRayCastingContext<THit> where THit : class
    {
        /// <summary>
        /// Performs a collision test between the ray and the element at the specified grid position.
        /// </summary>
        /// <param name="gridPosition">The grid position</param>
        /// <param name="position">The high resolution position</param>
        /// <param name="ray">The ray</param>
        /// <param name="hit">The hit if successful, otherwise null</param>
        /// <returns>Whether the ray hit something or not</returns>
        bool TryCollide(Vector3I gridPosition, Vector3 position, RayCaster.Ray ray, [NotNullWhen(true)] out THit? hit);
    }
}