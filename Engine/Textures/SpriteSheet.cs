using System.Collections.Generic;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Textures
{
    public sealed class SpriteSheet
    {
        public Texture Texture { get; }
        public IEnumerable<ISprite> Sprites { get; }

        public SpriteSheet(Texture texture, IEnumerable<ISprite> sprites)
        {
            Texture = texture;
            Sprites = sprites;
        }
    }
}