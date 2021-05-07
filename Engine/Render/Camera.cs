using System;
using System.Numerics;

namespace DigBuild.Engine.Render
{
    public class Camera : ICamera
    {
        public float FieldOfView { get; }

        public Vector3 Position { get; }

        public float Pitch { get; }
        public float Yaw { get; }

        public Camera(Vector3 position, float pitch, float yaw, float fieldOfView)
        {
            Position = position;
            Pitch = pitch;
            Yaw = yaw;
            FieldOfView = fieldOfView;
        }

        public Matrix4x4 Transform =>
            Matrix4x4.CreateTranslation(-Position) *
            Matrix4x4.CreateRotationY(MathF.PI - Yaw) *
            Matrix4x4.CreateRotationX(-Pitch);

        public Matrix4x4 FlattenTransform =>
            Matrix4x4.CreateRotationX(Pitch) *
            Matrix4x4.CreateRotationY(-MathF.PI + Yaw);
        
        public Vector3 Forward => Vector3.TransformNormal(
            Vector3.UnitZ,
            Matrix4x4.CreateRotationX(-Pitch)
            * Matrix4x4.CreateRotationY(Yaw)
        );
        public Vector3 Up => Vector3.TransformNormal(
            Vector3.UnitY,
            Matrix4x4.CreateRotationX(-Pitch)
            * Matrix4x4.CreateRotationY(Yaw)
        );
    }
}