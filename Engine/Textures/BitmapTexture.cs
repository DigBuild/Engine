using System.Drawing;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    public sealed class BitmapTexture : IBitmapTexture
    {
        private readonly IResource _resource;

        public ResourceName Name => _resource.Name;

        public Bitmap Bitmap => new(_resource.OpenStream());

        public BitmapTexture(IResource resource)
        {
            _resource = resource;
        }
    }
}