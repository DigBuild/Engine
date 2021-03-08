using System.Numerics;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Render
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Matrix4x4 Transform { get; }
        RayCaster.Ray Ray { get; }

        Vector3 Forward { get; }
        Vector3 Up { get; }
        float FieldOfView { get; }
    }
}