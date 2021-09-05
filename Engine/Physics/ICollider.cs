using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    /// <summary>
    /// A physics collider.
    /// </summary>
    public interface ICollider
    {
        /// <summary>
        /// An empty collider.
        /// </summary>
        static ICollider None { get; } = new NoCollider();
        
        /// <summary>
        /// Performs a collision test between this collider and the target AABB travelling at the given velocity.
        /// </summary>
        /// <param name="target">The target AABB</param>
        /// <param name="velocity">The target's velocity</param>
        /// <param name="delta">A 0-1 float indicating how far the AABB travelled before colliding</param>
        /// <param name="intersection">The intersection vector between the AABB and this geometry</param>
        /// <returns>Whether the collision was successful or not</returns>
        bool Collide(AABB target, Vector3 velocity, out float delta, out Vector3 intersection);

        private sealed class NoCollider : ICollider
        {
            public bool Collide(AABB target, Vector3 velocity, out float delta, out Vector3 intersection)
            {
                delta = 0;
                intersection = Vector3.Zero;
                return false;
            }
        }
    }
}