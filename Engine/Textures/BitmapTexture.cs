using System.Drawing;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Textures
{
    /// <summary>
    /// A custom resource type for bitmaps.
    /// </summary>
    public sealed class BitmapTexture : IBitmapTexture, ICustomResource
    {
        private readonly IResource _resource;

        public ResourceName Name => _resource.Name;

        public Bitmap Bitmap => new(_resource.OpenStream());

        public BitmapTexture(IResource resource)
        {
            _resource = resource;
        }

        public static BitmapTexture? Load(ResourceManager manager, ResourceName name)
        {
            if (manager.TryGetResource(name, out var res))
                return new BitmapTexture(res);
            return null;
        }
    }
}