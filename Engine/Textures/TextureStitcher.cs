using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    public sealed class TextureStitcher
    {
        private const int SpriteSize = 16;

        private readonly Dictionary<ResourceName, SpriteInfo> _sprites = new();

        public ISprite Add(IResource resource)
        {
            return Add(new BitmapTexture(resource));
        }

        public ISprite Add(IBitmapTexture texture)
        {
            if (_sprites.TryGetValue(texture.Name, out var currentInfo))
                return currentInfo.Sprite;

            var info = _sprites[texture.Name] = new SpriteInfo(texture);
            return info.Sprite;
        }

        public SpriteSheet Stitch(ResourceName name, string? outputFile = null)
        {
            if (_sprites.Count == 0)
                throw new Exception("Cannot build spritesheet without any sprites.");
            
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

                graphics.DrawImageUnscaledAndClipped(info.Texture.Bitmap, new Rectangle(x * SpriteSize, y * SpriteSize, SpriteSize, SpriteSize));

                x = (x + 1) % tiles;
                y += x == 0 ? 1 : 0;
            }

            if (outputFile != null)
                bitmap.Save(outputFile);

            return new SpriteSheet(name, bitmap, sprites);
        }

        private sealed class SpriteInfo
        {
            internal readonly IBitmapTexture Texture;

            internal readonly Sprite Sprite = new();

            public SpriteInfo(IBitmapTexture texture)
            {
                Texture = texture;
            }
        }

        private sealed class Sprite : ISprite
        {
            public Vector2 UV { get; internal set; }
            public Vector2 Size { get; internal set; }
        }
    }
}