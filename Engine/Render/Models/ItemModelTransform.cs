using System;
using System.Numerics;

namespace DigBuild.Engine.Render.Models
{
    public enum ItemModelTransform
    {
        None,
        Inventory
    }

    public static class ItemModelTransformExtensions
    {
        public static Matrix4x4 Ortho { get; } = Matrix4x4.CreateRotationY(-MathF.PI / 4, Vector3.One / 2) *
                                                 Matrix4x4.CreateRotationX(-MathF.Asin(1 / MathF.Sqrt(3)), Vector3.One / 2);

        public static Matrix4x4 GetMatrix(this ItemModelTransform transform)
        {
            return transform switch
            {
                ItemModelTransform.None => Matrix4x4.Identity,
                ItemModelTransform.Inventory => Ortho,
                _ => throw new ArgumentOutOfRangeException(nameof(transform))
            };
        }
    }
}