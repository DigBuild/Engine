using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public interface ICollider
    {
        static ICollider None { get; } = new NoCollider();

        bool Collide(Vector3 target, Vector3 motion, out Vector3 intersection);

        bool Collide(AABB target, Vector3 motion, out Vector3 intersection);

        private sealed class NoCollider : ICollider
        {
            public bool Collide(Vector3 target, Vector3 motion, out Vector3 intersection)
            {
                intersection = Vector3.Zero;
                return false;
            }

            public bool Collide(AABB target, Vector3 motion, out Vector3 intersection)
            {
                intersection = Vector3.Zero;
                return false;
            }
        }
    }
}