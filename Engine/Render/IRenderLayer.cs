using System;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render
{
    public interface IRenderLayer
    {
        void InitResources(RenderContext context, ResourceManager resourceManager, RenderStage renderStage);
        void SetupCommand(CommandBufferRecorder cmd, IReadOnlyUniformBufferSet uniforms, IReadOnlyTextureSet textures);
    }

    public interface IRenderLayer<TVertex> : IRenderLayer where TVertex : unmanaged
    {
        IVertexConsumer<TVertex> CreateTransformer(IVertexConsumer<TVertex> consumer, Matrix4x4 transform, bool transformNormal);
        IVertexConsumer<TVertex> CreateLightingTransformer(IVertexConsumer<TVertex> consumer, Func<Vector3, Vector3, float> lightValueProvider);

        void Draw(CommandBufferRecorder cmd, IReadOnlyUniformBufferSet uniforms, VertexBuffer<TVertex> vertexBuffer);
    }
}