using System.Numerics;
using DigBuild.Engine.BuiltIn.GeneratedUniforms;
using DigBuild.Engine.Render;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInRenderUniforms
    {
        public static RenderUniform<SimpleTransform> ModelViewTransform { get; } = new();
    }

    public interface ISimpleTransform : IUniform<SimpleTransform>
    {
        public Matrix4x4 Matrix { get; set; }
    }
}