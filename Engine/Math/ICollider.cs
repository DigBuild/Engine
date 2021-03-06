using System.Numerics;

namespace DigBuild.Engine.Math
{
    public interface ICollider
    {
        bool Collide(AABB target, Vector3 motion, out Vector3 intersection);
    }
}