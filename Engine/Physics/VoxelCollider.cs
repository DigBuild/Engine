using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public sealed class VoxelCollider : ICollider
    {
        private readonly AABB _bounds;
        
        public VoxelCollider(AABB bounds)
        {
            _bounds = bounds;
        }

        public bool Collide(AABB target, Vector3 motion, out Vector3 intersection)
        {
            var minkowski = AABB.MinkowskiDifference(target, _bounds);

            // Already intersecting
            if (minkowski.Contains(-motion, out intersection))
                return true;

            // Will intersect at some point
            if (minkowski.IntersectRay(Vector3.Zero, -motion, out var delta, out var side))
            {
                intersection = -motion * side.GetAxis().AsVector() * (1 - delta);
                return true;
            }

            // Won't intersect
            intersection = Vector3.Zero;
            return false;
        }
    }
}