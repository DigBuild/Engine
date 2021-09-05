using System;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A render layer.
    /// </summary>
    public interface IRenderLayer
    {
        /// <summary>
        /// Initializes the render resources for this layer.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="resourceManager">The resource manager</param>
        /// <param name="renderStage">The render stage</param>
        void InitResources(RenderContext context, ResourceManager resourceManager, RenderStage renderStage);
        /// <summary>
        /// Initializes the layer bindings.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="bindings">The layer bindings</param>
        void InitBindings(RenderContext context, RenderLayerBindingSet bindings);
        /// <summary>
        /// Sets up the draw command for this layer.
        /// </summary>
        /// <param name="cmd">The command buffer recorder</param>
        /// <param name="bindings">The layer bindings</param>
        /// <param name="uniforms">The uniform buffer set</param>
        /// <param name="textures">The texture set</param>
        void SetupCommand(CommandBufferRecorder cmd, RenderLayerBindingSet bindings, IReadOnlyUniformBufferSet uniforms, IReadOnlyTextureSet textures);
    }

    /// <summary>
    /// A render layer.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type</typeparam>
    public interface IRenderLayer<TVertex> : IRenderLayer
        where TVertex : unmanaged
    {
        /// <summary>
        /// Wraps a vertex consumer to apply the specified transform to all incoming vertices.
        /// </summary>
        /// <param name="consumer">The vertex consumer</param>
        /// <param name="transform">The transform matrix</param>
        /// <param name="transformNormal">Whether to also transform normals or not</param>
        /// <returns>The new vertex consumer</returns>
        IVertexConsumer<TVertex> CreateTransformer(IVertexConsumer<TVertex> consumer, Matrix4x4 transform, bool transformNormal);

        /// <summary>
        /// Wraps a vertex consumer to apply the specified light transfer function to all incoming vertices.
        /// </summary>
        /// <param name="consumer">The vertex consumer</param>
        /// <param name="lightValueProvider">The light transfer function</param>
        /// <returns>The new vertex consumer</returns>
        IVertexConsumer<TVertex> CreateLightingTransformer(IVertexConsumer<TVertex> consumer, Func<Vector3, Vector3, float> lightValueProvider);

        /// <summary>
        /// Draws the contents of the vertex buffer using this layer.
        /// </summary>
        /// <param name="cmd">The command buffer recorder</param>
        /// <param name="bindings">The layer bindings</param>
        /// <param name="uniforms">The uniforms</param>
        /// <param name="vertexBuffer">The vertex buffer</param>
        void Draw(CommandBufferRecorder cmd, RenderLayerBindingSet bindings, IReadOnlyUniformBufferSet uniforms, VertexBuffer<TVertex> vertexBuffer);
    }

    /// <summary>
    /// A render layer.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type</typeparam>
    /// <typeparam name="TBindings">The bindings type</typeparam>
    public interface IRenderLayer<TVertex, TBindings> : IRenderLayer<TVertex>
        where TVertex : unmanaged
    {
    }
}