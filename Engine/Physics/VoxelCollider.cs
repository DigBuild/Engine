using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    /// <summary>
    /// A basic single voxel collider.
    /// </summary>
    public sealed class VoxelCollider : ICollider
    {
        /// <summary>
        /// The bounds of this collider.
        /// </summary>
        public AABB Voxel { get; }

        public VoxelCollider(AABB voxel)
        {
            Voxel = voxel;
        }

        public bool Collide(AABB target, Vector3 velocity, out float delta, out Vector3 intersection)
        {
            var minkowski = AABB.MinkowskiDifference(target, Voxel);

            // Already intersecting
            if (minkowski.Contains(-velocity, out intersection))
            {
                delta = 0;
                return true;
            }
            
            // Will intersect at some point
            if (minkowski.IntersectRay(Vector3.Zero, -velocity, out delta, out var side))
            {
                intersection = -velocity * side.GetAxis().AsVector() * (1 - delta);
                return true;
            }

            // Won't intersect
            intersection = Vector3.Zero;
            return false;
        }
    }
}