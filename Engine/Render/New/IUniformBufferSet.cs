using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render.New
{
    public interface IUniformBufferSet : IReadOnlyUniformBufferSet
    {
        void Push<TUniform>(RenderUniform<TUniform> uniform, TUniform value)
            where TUniform : unmanaged, IUniform<TUniform>;
    }
}