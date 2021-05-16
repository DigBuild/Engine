using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public interface IRenderUniform
    {
        internal UniformBufferSet.IData CreateData(NativeBufferPool bufferPool);
    }

    public sealed class RenderUniform<TUniform> : IRenderUniform
        where TUniform : unmanaged, IUniform<TUniform>
    {
        UniformBufferSet.IData IRenderUniform.CreateData(NativeBufferPool bufferPool)
        {
            return new UniformBufferSet.Data<TUniform>(bufferPool);
        }
    }
}