using System.Numerics;

namespace DigBuild.Engine.Render.Worlds
{
    public readonly struct WorldView
    {
        public Matrix4x4 Projection { get; }
        public ICamera Camera { get; }
        public ViewFrustum ViewFrustum { get; }

        public WorldView(Matrix4x4 projection, ICamera camera, ViewFrustum viewFrustum)
        {
            Projection = projection;
            Camera = camera;
            ViewFrustum = viewFrustum;
        }
    }
}