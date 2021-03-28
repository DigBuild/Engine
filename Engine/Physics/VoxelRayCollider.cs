using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public sealed class VoxelRayCollider : IRayCollider<VoxelRayCollider.Hit>
    {
        private readonly AABB[] _bounds;

        public VoxelRayCollider(params AABB[] bounds)
        {
            _bounds = bounds;
        }

        public bool TryCollide(Raycast.Ray ray, [NotNullWhen(true)] out Hit? hit)
        {
            var delta = float.MaxValue;
            var side = Direction.NegX;
            var index = 0u;
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
                i++;
            }

            if (delta == float.MaxValue)
            {
                hit = null;
                return false;
            }
            hit = new Hit(delta, side, index);
            return true;
        }

        public IEnumerable<AABB> GetCollisionBoxes()
        {
            return _bounds;
        }

        public sealed class Hit
        {
            public readonly float Delta;
            public readonly Direction Side;
            public readonly uint Index;

            public Hit(float delta, Direction side, uint index)
            {
                Delta = delta;
                Side = side;
                Index = index;
            }
        }
    }
}