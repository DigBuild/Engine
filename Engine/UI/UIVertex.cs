using System.Numerics;
using DigBuild.Engine.Render;

namespace DigBuild.Engine.Ui
{
    public readonly struct UiVertex
    {
        public readonly Vector2 Position;
        public readonly Vector2 Uv;
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

        public static VertexTransformer<UiVertex> CreateTransformer(IVertexConsumer<UiVertex> next, Matrix4x4 transform, bool transformNormal)
        {
            return new(next, v => new UiVertex(
                Vector2.Transform(v.Position, transform),
                v.Uv,
                v.Color
            ));
        }
    }
}