using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    /// <summary>
    /// A basic multi-voxel ray collider.
    /// </summary>
    public sealed class VoxelRayCollider : IRayCollider<VoxelRayCollider.Hit>
    {
        private readonly AABB[] _bounds;

        /// <summary>
        /// The boxes that are part of this collider.
        /// </summary>
        public IEnumerable<AABB> Voxels => _bounds;

        public VoxelRayCollider(params AABB[] bounds)
        {
            _bounds = bounds;
        }

        public bool TryCollide(RayCaster.Ray ray, [NotNullWhen(true)] out Hit? hit)
        {
            var delta = float.MaxValue;
            var side = Direction.NegX;
            var index = 0u;
            AABB? hitBox = null;
            var i = 0u;
            foreach (var aabb in _bounds)
            {
                if (!aabb.IntersectRay(ray.Origin, ray.Magnitude, out var d, out var s) || d >= delta)
                {
                    i++;
                    continue;
                }
                delta = d;
                side = s;
                index = i;
                hitBox = aabb;
                i++;
            }

            if (!hitBox.HasValue)
            {
                hit = null;
                return false;
            }
            hit = new Hit(delta, side, index, hitBox.Value);
            return true;
        }

        public sealed class Hit
        {
            public readonly float Delta;
            public readonly Direction Side;
            public readonly uint Index;
            public readonly AABB Bounds;

            public Hit(float delta, Direction side, uint index, AABB bounds)
            {
                Delta = delta;
                Side = side;
                Index = index;
                Bounds = bounds;
            }
        }
    }
}