using System.Numerics;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A particle renderer.
    /// </summary>
    public interface IParticleRenderer
    {
        /// <summary>
        /// Initializes the render resources for this particle renderer.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="stage">The render stage</param>
        void Initialize(RenderContext context, RenderStage stage);

        /// <summary>
        /// Updates all the GPU particles.
        /// </summary>
        /// <param name="partialTick">The tick delta</param>
        void Update(float partialTick);

        /// <summary>
        /// Draws all the particles.
        /// </summary>
        /// <param name="cmd">The command buffer recorder</param>
        /// <param name="projection">The projection matrix</param>
        /// <param name="flattenTransform">The flatten transform</param>
        /// <param name="partialTick">The tick delta</param>
        void Draw(CommandBufferRecorder cmd, Matrix4x4 projection, Matrix4x4 flattenTransform, float partialTick);
    }
}