using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A uniform handle.
    /// </summary>
    public interface IUniformType
    {
        internal UniformBufferSet.IData CreateData(NativeBufferPool bufferPool);
    }
    
    /// <summary>
    /// A uniform handle.
    /// </summary>
    public sealed class UniformType<TUniform> : IUniformType
        where TUniform : unmanaged, IUniform<TUniform>
    {
        UniformBufferSet.IData IUniformType.CreateData(NativeBufferPool bufferPool)
        {
            return new UniformBufferSet.Data<TUniform>(bufferPool);
        }
    }
}