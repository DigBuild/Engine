using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public interface ICollider
    {
        bool Collide(AABB target, Vector3 motion, out Vector3 intersection);
    }
}