using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render
{
    public interface IRenderLayer
    {
        void InitResources(RenderContext context, ResourceManager resourceManager, RenderStage renderStage);
        void InitBindings(RenderContext context, RenderLayerBindingSet bindings);
        void SetupCommand(CommandBufferRecorder cmd, RenderLayerBindingSet bindings, IReadOnlyUniformBufferSet uniforms, IReadOnlyTextureSet textures);
    }

    public interface IRenderLayer<TVertex> : IRenderLayer
        where TVertex : unmanaged
    {
        IVertexConsumer<TVertex> CreateTransformer(IVertexConsumer<TVertex> consumer, Matrix4x4 transform, bool transformNormal);
        IVertexConsumer<TVertex> CreateLightingTransformer(IVertexConsumer<TVertex> consumer, Func<Vector3, Vector3, float> lightValueProvider);

        void Draw(CommandBufferRecorder cmd, RenderLayerBindingSet bindings, IReadOnlyUniformBufferSet uniforms, VertexBuffer<TVertex> vertexBuffer);
    }

    public interface IRenderLayer<TVertex, TBindings> : IRenderLayer<TVertex>
        where TVertex : unmanaged
    {
    }

    public sealed class RenderLayerBindingSet
    {
        private readonly Dictionary<IRenderLayer, object> _bindings = new();

        public void Set<TVertex, TBindings>(IRenderLayer<TVertex, TBindings> layer, TBindings bindings)
            where TVertex : unmanaged
        {
            _bindings[layer] = bindings!;
        }

        public TBindings Get<TVertex, TBindings>(IRenderLayer<TVertex, TBindings> layer)
            where TVertex : unmanaged
        {
            return (TBindings)_bindings[layer];
        }
    }
}