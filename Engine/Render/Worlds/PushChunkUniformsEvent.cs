using DigBuild.Engine.Events;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Render.Worlds
{
    public sealed class PushChunkUniformsEvent : IEvent
    {
        public IReadOnlyChunk Chunk { get; }
        public IUniformBufferSetWriter Uniforms { get; }

        public PushChunkUniformsEvent(IReadOnlyChunk chunk, IUniformBufferSetWriter uniforms)
        {
            Chunk = chunk;
            Uniforms = uniforms;
        }
    }
}