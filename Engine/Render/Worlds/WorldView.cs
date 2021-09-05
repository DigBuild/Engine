using System.Numerics;

namespace DigBuild.Engine.Render.Worlds
{
    /// <summary>
    /// World view and transform information.
    /// </summary>
    public readonly struct WorldView
    {
        /// <summary>
        /// The projection matrix.
        /// </summary>
        public Matrix4x4 Projection { get; }
        /// <summary>
        /// The camera.
        /// </summary>
        public ICamera Camera { get; }
        /// <summary>
        /// The view frustum.
        /// </summary>
        public ViewFrustum ViewFrustum { get; }

        public WorldView(Matrix4x4 projection, ICamera camera, ViewFrustum viewFrustum)
        {
            Projection = projection;
            Camera = camera;
            ViewFrustum = viewFrustum;
        }
    }
}