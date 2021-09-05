using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A view frustum composed by 6 planes.
    /// </summary>
    public sealed class ViewFrustum
    {
        private readonly Plane[] _planes = new Plane[6];

        public ViewFrustum(Matrix4x4 viewProj)
        {
            _planes[0] = Plane.Normalize(new Plane(
                viewProj.M14 + viewProj.M11,
                viewProj.M24 + viewProj.M21,
                viewProj.M34 + viewProj.M31,
                viewProj.M44 + viewProj.M41
            ));
            _planes[1] = Plane.Normalize(new Plane(
                viewProj.M14 - viewProj.M11,
                viewProj.M24 - viewProj.M21,
                viewProj.M34 - viewProj.M31,
                viewProj.M44 - viewProj.M41
            ));
            _planes[2] = Plane.Normalize(new Plane(
                viewProj.M14 + viewProj.M12,
                viewProj.M24 + viewProj.M22,
                viewProj.M34 + viewProj.M32,
                viewProj.M44 + viewProj.M42
            ));
            _planes[3] = Plane.Normalize(new Plane(
                viewProj.M14 - viewProj.M12,
                viewProj.M24 - viewProj.M22,
                viewProj.M34 - viewProj.M32,
                viewProj.M44 - viewProj.M42
            ));
            _planes[4] = Plane.Normalize(new Plane(
                viewProj.M14 + viewProj.M13,
                viewProj.M24 + viewProj.M23,
                viewProj.M34 + viewProj.M33,
                viewProj.M44 + viewProj.M43
            ));
            _planes[5] = Plane.Normalize(new Plane(
                viewProj.M14 - viewProj.M13,
                viewProj.M24 - viewProj.M23,
                viewProj.M34 - viewProj.M33,
                viewProj.M44 - viewProj.M43
            ));
        }

        /// <summary>
        /// Checks if the AABB falls within the bounds of this view frustum.
        /// </summary>
        /// <param name="aabb">The AABB</param>
        /// <returns>Whether it is within the view frustum or not</returns>
        public bool Test(AABB aabb)
        {
            foreach (var plane in _planes)
            {
                var x = plane.Normal.X < 0 ? aabb.Min.X : aabb.Max.X;
                var y = plane.Normal.Y < 0 ? aabb.Min.Y : aabb.Max.Y;
                var z = plane.Normal.Z < 0 ? aabb.Min.Z : aabb.Max.Z;

                if (Plane.DotCoordinate(plane, new Vector3(x, y, z)) < 0)
                    return false;
            }
            return true;
        }
    }
}