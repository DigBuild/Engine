using System.Numerics;
using DigBuild.Engine.Render;

namespace DigBuild.Engine.UI
{
    public readonly struct UIVertex
    {
        public readonly Vector2 Position;
        public readonly Vector2 Uv;

        public UIVertex(Vector2 position, Vector2 uv)
        {
            Position = position;
            Uv = uv;
        }

        public UIVertex(float x, float y, float u, float v)
        {
            Position = new Vector2(x, y);
            Uv = new Vector2(u, v);
        }

        public static VertexTransformer<UIVertex> CreateTransformer(IVertexConsumer<UIVertex> next, Matrix4x4 transform)
        {
            return new(next, v => new UIVertex(
                Vector2.Transform(v.Position, transform),
                v.Uv
            ));
        }
    }
}