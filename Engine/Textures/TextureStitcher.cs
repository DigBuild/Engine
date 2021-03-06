using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    public sealed class TextureStitcher
    {
        private const int SpriteSize = 16;

        private readonly Dictionary<IResource, SpriteInfo> _sprites = new();
        private bool _built;

        public ISprite Add(IResource texture)
        {
            var info = _sprites[texture] = new SpriteInfo(texture);
            return info.Sprite;
        }

        public SpriteSheet Build(RenderContext context)
        {
            if (_built)
                throw new Exception("Attempted to build more than once.");
            if (_sprites.Count == 0)
                throw new Exception("Cannot build spritesheet without any sprites.");
            _built = true;
            
            var tiles = (int) System.Math.Pow(2, System.Math.Ceiling(System.Math.Log2(_sprites.Count)));
            var size = SpriteSize * tiles;
            var relativeSpriteSize = Vector2.One / tiles;

            var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);

            var sprites = new List<ISprite>(_sprites.Count);

            int x = 0, y = 0;
            foreach (var info in _sprites.Values)
            {
                info.Sprite.UV = new Vector2(x, y) / tiles;
                info.Sprite.Size = relativeSpriteSize;

                sprites.Add(info.Sprite);

                graphics.DrawImageUnscaledAndClipped(info.Bitmap, new Rectangle(x * SpriteSize, y * SpriteSize, SpriteSize, SpriteSize));

                x = (x + 1) % tiles;
                y += x == 0 ? 1 : 0;
            }

            bitmap.Save("spritesheet.png");
            
            _sprites.Clear();

            var texture = context.CreateTexture(bitmap);
            return new SpriteSheet(texture, sprites);
        }

        private sealed class SpriteInfo
        {
            internal readonly Sprite Sprite = new();
            internal readonly Bitmap Bitmap;

            public SpriteInfo(IResource texture)
            {
                Bitmap = new Bitmap(texture.OpenStream());
                if (Bitmap.Width != Bitmap.Height)
                    throw new ArgumentException($"Texture must be square: {texture.Name}", nameof(texture));
                if (Bitmap.Width != SpriteSize)
                    throw new ArgumentException($"Texture must be 16x16: {texture.Name}", nameof(texture));
            }
        }

        private sealed class Sprite : ISprite
        {
            public Vector2 UV { get; internal set; }
            public Vector2 Size { get; internal set; }
        }
    }
}