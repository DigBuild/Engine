using System.Numerics;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A 3D camera.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// The position.
        /// </summary>
        Vector3 Position { get; }
        /// <summary>
        /// The view transform.
        /// </summary>
        Matrix4x4 Transform { get; }
        /// <summary>
        /// A transform that flattens geometry.
        /// </summary>
        Matrix4x4 FlattenTransform { get; }

        /// <summary>
        /// The forward vector.
        /// </summary>
        Vector3 Forward { get; }
        /// <summary>
        /// The up vector.
        /// </summary>
        Vector3 Up { get; }
        /// <summary>
        /// The field of view.
        /// </summary>
        float FieldOfView { get; }
    }
}