using System;
using System.Numerics;

namespace DigBuild.Engine.Render.Models
{
    /// <summary>
    /// An item model transform.
    /// </summary>
    public enum ItemModelTransform
    {
        None,
        Inventory
    }

    /// <summary>
    /// Item model transform helpers.
    /// </summary>
    public static class ItemModelTransformExtensions
    {
        /// <summary>
        /// An isometric view matrix for orthographic projections.
        /// </summary>
        public static Matrix4x4 Isometric { get; } = Matrix4x4.CreateRotationY(-MathF.PI / 4, Vector3.One / 2) *
                                                     Matrix4x4.CreateRotationX(-MathF.Asin(1 / MathF.Sqrt(3)), Vector3.One / 2);

        /// <summary>
        /// Gets the matrix for this transform.
        /// </summary>
        /// <param name="transform">The transform</param>
        /// <returns>The matrix</returns>
        public static Matrix4x4 GetMatrix(this ItemModelTransform transform)
        {
            return transform switch
            {
                ItemModelTransform.None => Matrix4x4.Identity,
                ItemModelTransform.Inventory => Isometric,
                _ => throw new ArgumentOutOfRangeException(nameof(transform))
            };
        }
    }
}