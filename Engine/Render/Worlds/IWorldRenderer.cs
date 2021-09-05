using System;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render.Worlds
{
    /// <summary>
    /// A world renderer.
    /// </summary>
    public interface IWorldRenderer : IDisposable
    {
        /// <summary>
        /// Updates and uploads any necessary geometry.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="worldView">The world view</param>
        /// <param name="partialTick">The partial tick</param>
        void Update(RenderContext context, WorldView worldView, float partialTick);
        
        /// <summary>
        /// Begins drawing. Useful for collecting uniforms and setting up state.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="cmd">The command buffer recorder</param>
        /// <param name="uniforms">The uniform set</param>
        /// <param name="worldView">The world view</param>
        /// <param name="partialTick">The tick delta</param>
        void BeforeDraw(RenderContext context, CommandBufferRecorder cmd, UniformBufferSet uniforms, WorldView worldView, float partialTick);
        /// <summary>
        /// Draws the contents of a given layer.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="cmd">The command buffer recorer</param>
        /// <param name="layer">The render layer</param>
        /// <param name="bindings">The layer bindings</param>
        /// <param name="uniforms">The uniforms</param>
        /// <param name="worldView">The world view</param>
        /// <param name="partialTick">The tick delta</param>
        void Draw(RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer, RenderLayerBindingSet bindings, IReadOnlyUniformBufferSet uniforms, WorldView worldView, float partialTick);
        /// <summary>
        /// Finishes drawing. Useful for clearing state.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="cmd">The command buffer recorer</param>
        /// <param name="worldView">The world view</param>
        /// <param name="partialTick">The tick delta</param>
        void AfterDraw(RenderContext context, CommandBufferRecorder cmd, WorldView worldView, float partialTick);
    }
}