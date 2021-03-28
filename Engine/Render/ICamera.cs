using System.Numerics;
using DigBuild.Engine.Physics;

namespace DigBuild.Engine.Render
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Matrix4x4 Transform { get; }
        Raycast.Ray Ray { get; }

        Vector3 Forward { get; }
        Vector3 Up { get; }
        float FieldOfView { get; }
    }
}