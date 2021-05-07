using System.Numerics;

namespace DigBuild.Engine.Render
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Matrix4x4 Transform { get; }
        Matrix4x4 FlattenTransform { get; }

        Vector3 Forward { get; }
        Vector3 Up { get; }
        float FieldOfView { get; }
    }
}