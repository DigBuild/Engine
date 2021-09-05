using System.Diagnostics.CodeAnalysis;

namespace DigBuild.Engine.Physics
{
    /// <summary>
    /// A ray collider.
    /// </summary>
    /// <typeparam name="THit">The returned hit type</typeparam>
    public interface IRayCollider<THit>
    {
        /// <summary>
        /// An empty ray collider.
        /// </summary>
        static IRayCollider<THit> None { get; } = new NoCollider();

        /// <summary>
        /// Performs a collision test between this collider and the ray.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <param name="hit">The hit result if successful, otherwise null</param>
        /// <returns>Whether the ray hit this collider or not</returns>
        bool TryCollide(RayCaster.Ray ray, [NotNullWhen(true)] out THit? hit);

        private sealed class NoCollider : IRayCollider<THit>
        {
            public bool TryCollide(RayCaster.Ray ray, [NotNullWhen(true)] out THit? hit)
            {
                hit = default;
                return false;
            }
        }
    }
}