using System.Numerics;
using DigBuild.Engine.BuiltIn.GeneratedUniforms;
using DigBuild.Engine.Render;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.BuiltIn
{
    /// <summary>
    /// The built-in render uniforms.
    /// </summary>
    public static class BuiltInRenderUniforms
    {
        /// <summary>
        /// A two-matrix model-view + projection transform.
        /// </summary>
        public static Render.UniformType<SimpleTransform> ModelViewProjectionTransform { get; } = new();
    }

    public interface ISimpleTransform : IUniform<SimpleTransform>
    {
        public Matrix4x4 ModelView { get; set; }
        public Matrix4x4 Projection { get; set; }
    }
}