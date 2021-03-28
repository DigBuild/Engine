using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Raycasting
{
    public sealed class VoxelRayCollider : IRayCollider
    {
        private readonly AABB[] _bounds;

        public VoxelRayCollider(params AABB[] bounds)
        {
            _bounds = bounds;
        }

        public IEnumerable<AABB> GetCollisionBoxes()
        {
            return _bounds;
        }
    }
}