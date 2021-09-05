using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    /// <summary>
    /// A simple multi-voxel collider.
    /// </summary>
    public sealed class MultiVoxelCollider : ICollider
    {
        private readonly List<VoxelCollider> _colliders = new();

        public IEnumerable<AABB> Voxels => _colliders.Select(c => c.Voxel);

        public MultiVoxelCollider(IEnumerable<AABB> aabbs)
        {
            foreach (var aabb in aabbs)
                _colliders.Add(new VoxelCollider(aabb));
        }

        public bool Collide(AABB target, Vector3 velocity, out float delta, out Vector3 intersection)
        {
            var success = false;
            intersection = Vector3.Zero;

            delta = 1;

            foreach (var collider in _colliders)
            {
                if (!collider.Collide(target, velocity, out var d, out var i))
                    continue;

                success = true;
                delta = MathF.Min(delta, d);
                intersection = new Vector3(
                    MathF.Abs(intersection.X) > MathF.Abs(i.X) ? intersection.X : i.X,
                    MathF.Abs(intersection.Y) > MathF.Abs(i.Y) ? intersection.Y : i.Y,
                    MathF.Abs(intersection.Z) > MathF.Abs(i.Z) ? intersection.Z : i.Z
                );
            }

            return success;
        }
    }
}