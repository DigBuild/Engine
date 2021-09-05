using System.Collections.Generic;
using System.Drawing;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    /// <summary>
    /// A spritesheet bitmap.
    /// </summary>
    public sealed class SpriteSheet : IBitmapTexture
    {
        public ResourceName Name { get; }
        public Bitmap Bitmap { get; }
        /// <summary>
        /// The sprites.
        /// </summary>
        public IEnumerable<ISprite> Sprites { get; }

        public SpriteSheet(ResourceName name, Bitmap bitmap, IEnumerable<ISprite> sprites)
        {
            Name = name;
            Bitmap = bitmap;
            Sprites = sprites;
        }
    }
}