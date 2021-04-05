using System.Drawing;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    public interface IBitmapTexture
    {
        ResourceName Name { get; }

        Bitmap Bitmap { get; }
    }
}