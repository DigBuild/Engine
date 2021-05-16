using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    public interface IUniformBufferSetWriter
    {
        uint Push<TUniform>(RenderUniform<TUniform> uniform, TUniform value)
            where TUniform : unmanaged, IUniform<TUniform>;
    }
}