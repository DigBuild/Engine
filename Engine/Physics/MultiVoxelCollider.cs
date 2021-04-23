using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public sealed class MultiVoxelCollider : ICollider
    {
        private readonly List<VoxelCollider> _colliders = new();

        public MultiVoxelCollider(IEnumerable<AABB> aabbs)
        {
            foreach (var aabb in aabbs)
                _colliders.Add(new VoxelCollider(aabb));
        }

        public bool Collide(AABB target, Vector3 motion, out Vector3 intersection)
        {
            var success = false;
            intersection = Vector3.Zero;

            foreach (var collider in _colliders)
            {
                if (!collider.Collide(target, motion, out var i))
                    continue;

                success = true;
                intersection = new Vector3(
                    MathF.Abs(intersection.X) > MathF.Abs(i.X) ? intersection.X : i.X,
                    MathF.Abs(intersection.Y) > MathF.Abs(i.Y) ? intersection.Y : i.Y,
                    MathF.Abs(intersection.Z) > MathF.Abs(i.Z) ? intersection.Z : i.Z
                );
            }

            return success;
        }

        public bool Collide(Vector3 target, Vector3 motion, out Vector3 intersection)
        {
            var success = false;
            intersection = Vector3.Zero;

            foreach (var collider in _colliders)
            {
                if (!collider.Collide(target, motion, out var i))
                    continue;

                success = true;
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