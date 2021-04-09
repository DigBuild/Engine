using DigBuild.Engine.Particles;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public interface IParticleRenderer
    {
        void Update();
        void Draw(CommandBufferRecorder cmd);
    }

    public class ParticleRenderer<TVertex, TGpu> : IParticleRenderer
        where TVertex : unmanaged
        where TGpu : unmanaged
    {
        private readonly IParticleSystem<TGpu> _particleSystem;
        private readonly RenderPipeline<TVertex, TGpu> _pipeline;
        private readonly VertexBuffer<TVertex> _vertexBuffer;
        private readonly VertexBuffer<TGpu> _instanceBuffer;
        private readonly VertexBufferWriter<TGpu> _instanceBufferWriter;

        public ParticleRenderer(
            RenderContext context,
            NativeBufferPool pool,
            IParticleSystem<TGpu> particleSystem,
            RenderPipeline<TVertex, TGpu> pipeline,
            params TVertex[] vertices
        )
        {
            _particleSystem = particleSystem;
            _pipeline = pipeline;

            using (var vertexNativeBuffer = pool.Request<TVertex>())
            {
                vertexNativeBuffer.Add(vertices);
                _vertexBuffer = context.CreateVertexBuffer(vertexNativeBuffer);
            }

            _instanceBuffer = context.CreateVertexBuffer(out _instanceBufferWriter);
        }

        public void Update()
        {
            _instanceBufferWriter.Write(_particleSystem.GpuBuffer);
        }

        public void Draw(CommandBufferRecorder cmd)
        {
            cmd.Draw(_pipeline, _vertexBuffer, _instanceBuffer);
        }
    }
}