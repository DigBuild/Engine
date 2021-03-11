using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Render;

namespace DigBuild.Engine.UI
{
    public sealed class TextRenderer
    {
        private readonly RenderLayer<TextVertex> _layer;
        private readonly Dictionary<char, CharacterInfo> _characters = new();

        public TextRenderer(RenderLayer<TextVertex> layer)
        {
            _layer = layer;
            for (var c = 'a'; c <= 'z'; c++)
                _characters[c] = new CharacterInfo(c);
            for (var c = 'A'; c <= 'Z'; c++)
                _characters[c] = new CharacterInfo(c);
            for (var c = '0'; c <= '9'; c++)
                _characters[c] = new CharacterInfo(c);
            
            _characters[':'] = new CharacterInfo(':');
            _characters['<'] = new CharacterInfo('<');
            _characters['>'] = new CharacterInfo('>');
            _characters[','] = new CharacterInfo(',');
            _characters['.'] = new CharacterInfo('.');
        }

        public uint GetWidth(char c)
        {
            return CharacterInfo.CharWidth;
        }

        public uint DrawLine(GeometryBufferSet buffers, string text, uint scale = 1)
        {
            var length = 0u;

            var transform = buffers.Transform;

            foreach (var c in text)
            {
                if (_characters.TryGetValue(c, out var info))
                {
                    buffers.Transform = Matrix4x4.CreateTranslation(length, 0, 0) *
                                        Matrix4x4.CreateScale(scale) *
                                        transform;
                    buffers.Get(_layer).Accept(info.Vertices);
                }

                length += GetWidth(c);
            }

            return length * scale;
        }

        private readonly struct CharacterInfo
        {
            public const uint CharWidth = 5;
            private const uint CharHeight = 8;
            private const uint CharBaseline = 0;
            private const uint TextureSize = 128;

            internal readonly TextVertex[] Vertices;

            public CharacterInfo(char c)
            {
                var (posX, posY) = GetCharacterPosition(c);
                
                var v1 = new TextVertex(
                    new Vector2(0, -CharBaseline),
                    new Vector2(posX * CharWidth, posY * CharHeight) / TextureSize
                );
                var v2 = new TextVertex(
                    new Vector2(CharWidth, -CharBaseline),
                    new Vector2((posX + 1) * CharWidth, posY * CharHeight) / TextureSize
                );
                var v3 = new TextVertex(
                    new Vector2(CharWidth, -CharBaseline + CharHeight),
                    new Vector2((posX + 1) * CharWidth, (posY + 1) * CharHeight) / TextureSize
                );
                var v4 = new TextVertex(
                    new Vector2(0, -CharBaseline + CharHeight),
                    new Vector2(posX * CharWidth, (posY + 1) * CharHeight) / TextureSize
                );

                Vertices = new[] { v1, v2, v3, v3, v4, v1 };
            }

            private static (uint, uint) GetCharacterPosition(char c)
            {
                if (c is >= 'a' and <= 'z')
                    c = (char) (c - 'a' + 'A');

                if (c is >= 'A' and <= 'Z')
                    return ((uint) ((c - 'A') % 25), (uint) ((c - 'A') / 25));

                if (c is >= '0' and <= '9')
                    return ((uint) (c - '0' + 1), 1u);
                
                if (c == ':') return (11u, 1u);
                if (c == '<') return (12u, 1u);
                if (c == '>') return (13u, 1u);
                if (c == ',') return (14u, 1u);
                if (c == '.') return (15u, 1u);

                return (0u, 3u);
            }
        }
    }

    public readonly struct TextVertex
    {
        public readonly Vector2 Position;
        public readonly Vector2 Uv;

        public TextVertex(Vector2 position, Vector2 uv)
        {
            Position = position;
            Uv = uv;
        }

        public static VertexTransformer<TextVertex> CreateTransformer(IVertexConsumer<TextVertex> next, Matrix4x4 transform)
        {
            return new(next, v => new TextVertex(
                Vector2.Transform(v.Position, transform),
                v.Uv
            ));
        }
    }
}