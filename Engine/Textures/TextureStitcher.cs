using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    /// <summary>
    /// A stitcher that generates sprites and spritesheets from bitmaps.
    /// </summary>
    public sealed class TextureStitcher
    {
        private readonly Dictionary<ResourceName, SpriteInfo> _sprites = new();

        /// <summary>
        /// Adds a bitmap resource and returns an uninitialized sprite.
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <returns>The sprite</returns>
        public ISprite Add(IResource resource)
        {
            return Add(new BitmapTexture(resource));
        }

        /// <summary>
        /// Adds a bitmap texture and returns an uninitialized sprite.
        /// </summary>
        /// <param name="texture">The texture</param>
        /// <returns>The sprite</returns>
        public ISprite Add(IBitmapTexture texture)
        {
            if (_sprites.TryGetValue(texture.Name, out var currentInfo))
                return currentInfo.Sprite;

            var bmp = texture.Bitmap;
            if (bmp.Width != bmp.Height)
                throw new ArgumentException("The sprite must be square.", nameof(texture));

            var setBits = BitOperations.PopCount((uint)bmp.Width);
            if (setBits != 1)
                throw new ArgumentException("The sprite size must be a power of 2.", nameof(texture));

            var info = _sprites[texture.Name] = new SpriteInfo(bmp);
            return info.Sprite;
        }

        /// <summary>
        /// Stitches all the textures together into a spritesheet and optionally outputs it to a file.
        /// </summary>
        /// <param name="name">The spritesheet name</param>
        /// <param name="outputFile">The output file</param>
        /// <returns>The spritesheet</returns>
        public SpriteSheet Stitch(ResourceName name, string? outputFile = null)
        {
            if (_sprites.Count == 0)
                throw new Exception("Cannot build spritesheet without any sprites.");

            var bitmaps = 
                _sprites.Values
                    .OrderByDescending(info => info.Bitmap.Width * info.Bitmap.Height)
                    .ToList();

            var bitmap = (Bitmap?)null;
            var graphics = (Graphics?)null;
            var emptyRegions = new Stack<Rectangle>();
            var sprites = new List<Sprite>(_sprites.Count);

            foreach (var info in bitmaps)
            {
                var bmp = info.Bitmap;

                // If this is the first texture, just set it as our bitmap and continue
                if (bitmap == null)
                {
                    bitmap = bmp;

                    info.Sprite.UV = Vector2.Zero;
                    info.Sprite.Size = Vector2.One;
                    sprites.Add(info.Sprite);

                    continue;
                }

                // Upscale if there is no room
                if (!emptyRegions.TryPop(out var region))
                {
                    graphics?.Dispose();

                    var oldSize = bitmap.Width;
                    var size = oldSize * 2;
                    var newBitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                    graphics = Graphics.FromImage(newBitmap);

                    // Re-draw current textures
                    graphics.DrawImageUnscaledAndClipped(bitmap, new Rectangle(0, 0, oldSize, oldSize));
                    
                    // Deal with the new regions
                    emptyRegions.Push(new Rectangle(oldSize, oldSize, oldSize, oldSize));
                    emptyRegions.Push(new Rectangle(0, oldSize, oldSize, oldSize));
                    region = new Rectangle(oldSize, 0, oldSize, oldSize);

                    // Scale all the existing UVs
                    foreach (var sprite in sprites)
                    {
                        sprite.UV /= 2;
                        sprite.Size /= 2;
                    }

                    // Update the bitmap
                    bitmap = newBitmap;
                }

                // Draw the texture to the region and add the sprite
                var bmpSize = bmp.Width;
                graphics!.DrawImageUnscaledAndClipped(bmp, new Rectangle(region.X, region.Y, bmpSize, bmpSize));
                
                info.Sprite.UV = new Vector2(region.X / (float) bitmap.Width, region.Y / (float) bitmap.Height);
                info.Sprite.Size = new Vector2(bmpSize / (float) bitmap.Width, bmpSize / (float) bitmap.Height);
                sprites.Add(info.Sprite);

                // Subdivide if our current texture is smaller than the region we're drawing to
                var subdiv = region.Width / bmpSize;
                if (subdiv <= 1)
                    continue;

                for (var y = subdiv - 1; y >= 0; y--)
                for (var x = subdiv - 1; x >= 0; x--)
                {
                    if (x == 0 && y == 0) continue;
                    emptyRegions.Push(new Rectangle(region.X + x * bmpSize, region.Y + y * bmpSize, bmpSize, bmpSize));
                }
            }
            
            if (outputFile != null)
                bitmap!.Save(outputFile);
            
            return new SpriteSheet(name, bitmap!, sprites);
        }

        private sealed class SpriteInfo
        {
            internal readonly Bitmap Bitmap;

            internal readonly Sprite Sprite = new();

            public SpriteInfo(Bitmap bitmap)
            {
                Bitmap = bitmap;
            }
        }

        private sealed class Sprite : ISprite
        {
            public Vector2 UV { get; internal set; }
            public Vector2 Size { get; internal set; }
        }
    }
}