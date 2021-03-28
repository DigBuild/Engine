using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Raycasting
{
    public interface IRayCollider
    {
        IEnumerable<AABB> GetCollisionBoxes();
    }
}