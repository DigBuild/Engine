using System.Drawing;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    /// <summary>
    /// A bitmap texture.
    /// </summary>
    public interface IBitmapTexture
    {
        /// <summary>
        /// The name.
        /// </summary>
        ResourceName Name { get; }

        /// <summary>
        /// The bitmap.
        /// </summary>
        Bitmap Bitmap { get; }
    }
}