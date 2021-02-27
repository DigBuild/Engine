using System.Numerics;

namespace DigBuildEngine.Math
{
    public interface ICollider
    {
        bool Collide(AABB target, Vector3 motion, out Vector3 intersection);
    }

    public sealed class VoxelCollider : ICollider
    {
        private readonly AABB _bounds;

        public VoxelCollider() : this(AABB.FullBlock) { }

        public VoxelCollider(AABB bounds)
        {
            _bounds = bounds;
        }

        public bool Collide(AABB target, Vector3 motion, out Vector3 intersection)
        {
            if (!target.Intersects(_bounds, out intersection))
                return false;
            intersection = GetLowestEffectiveComponent(intersection, motion);
            return true;
        }

        private static Vector3 GetLowestEffectiveComponent(Vector3 intersection, Vector3 velocity)
        {
            var a = Vector3.Abs(ReplaceNaN(intersection / velocity, float.MaxValue));
            var (ax, ay, az) = (a.X, a.Y, a.Z);
            if (ax < ay)
                return ax < az ? new Vector3(intersection.X, 0, 0) : new Vector3(0, 0, intersection.Z);
            else
                return az < ay ? new Vector3(0, 0, intersection.Z) : new Vector3(0, intersection.Y, 0);
        }
        
        public static Vector3 ReplaceNaN(Vector3 vec, float value)
        {
            return new(
                !float.IsNaN(vec.X) ? vec.X : value,
                !float.IsNaN(vec.Y) ? vec.Y : value,
                !float.IsNaN(vec.Z) ? vec.Z : value
            );
        }
    }
}