using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render.New
{
    public interface IRenderLayer<TVertex> where TVertex : unmanaged
    {
        IVertexConsumer<TVertex> CreateTransformer(IVertexConsumer<TVertex> consumer, Matrix4x4 transform, bool transformNormal);

        void InitResources(RenderContext context, ResourceManager resourceManager, RenderStage renderStage);

        void SetupCommand(CommandBufferRecorder cmd, IReadOnlyUniformBufferSet uniforms, IReadOnlyTextureSet textures);
        void Draw(CommandBufferRecorder cmd, IReadOnlyUniformBufferSet uniforms, VertexBuffer<TVertex> vertexBuffer);
    }
}