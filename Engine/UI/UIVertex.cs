using System.Numerics;
using DigBuild.Engine.Render;

namespace DigBuild.Engine.Ui
{
    /// <summary>
    /// User interface vertex format.
    /// </summary>
    public readonly struct UiVertex
    {
        /// <summary>
        /// The position.
        /// </summary>
        public readonly Vector2 Position;
        /// <summary>
        /// The UV.
        /// </summary>
        public readonly Vector2 Uv;
        /// <summary>
        /// The color.
        /// </summary>
        public readonly Vector4 Color;

        public UiVertex(Vector2 position, Vector2 uv, Vector4 color)
        {
            Position = position;
            Uv = uv;
            Color = color;
        }

        public UiVertex(float x, float y, float u, float v, float r, float g, float b, float a)
        {
            Position = new Vector2(x, y);
            Uv = new Vector2(u, v);
            Color = new Vector4(r, g, b, a);
        }

        /// <summary>
        /// Creates a vertex consumer that applies the given transform before passing data on to the specified consumer.
        /// </summary>
        /// <param name="next">The following consumer in the chain</param>
        /// <param name="transform">The transform matrix</param>
        /// <param name="transformNormal">Whether to transform normals or not</param>
        /// <returns>The new vertex consumer</returns>
        public static IVertexConsumer<UiVertex> CreateTransformer(IVertexConsumer<UiVertex> next, Matrix4x4 transform, bool transformNormal)
        {
            return new VertexTransformer<UiVertex>(next, v => new UiVertex(
                Vector2.Transform(v.Position, transform),
                v.Uv,
                v.Color
            ));
        }
    }
}