using System.Numerics;
using DigBuild.Engine.Render;
using DigBuild.Engine.Textures;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    public sealed class UiRectangle : IUiElement
    {
        private readonly IRenderLayer<UiVertex> _layer;
        private readonly UiVertex[] _vertices;

        public UiRectangle(uint width, uint height, IRenderLayer<UiVertex> layer, ISprite sprite, Vector4 color)
        {
            _layer = layer;
            
            var v1 = new UiVertex(
                new Vector2(0, 0),
                sprite.GetInterpolatedUV(0, 0),
                color
            );
            var v2 = new UiVertex(
                new Vector2(width, 0),
                sprite.GetInterpolatedUV(1, 0),
                color
            );
            var v3 = new UiVertex(
                new Vector2(width, height),
                sprite.GetInterpolatedUV(1, 1),
                color
            );
            var v4 = new UiVertex(
                new Vector2(0, height),
                sprite.GetInterpolatedUV(0, 1),
                color
            );

            _vertices = new[] { v1, v2, v3, v3, v4, v1 };
        }

        public void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick)
        {
            var buf = buffer.Get(_layer);
            buf.Accept(_vertices);
        }
    }
}