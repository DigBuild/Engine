using System.Numerics;

namespace DigBuildEngine.Render
{
    public interface ICamera
    {
        Matrix4x4 Transform { get; }
        Vector3 Position { get; }

        Matrix4x4 GetInterpolatedTransform(float partialTick);
    }
}