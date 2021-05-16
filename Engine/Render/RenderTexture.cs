using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render
{
    public sealed class RenderTexture
    {
        public ResourceName Name { get; }
        
        public RenderTexture(ResourceName name)
        {
            Name = name;
        }

        public RenderTexture(string domain, string path) : this(new ResourceName(domain, path))
        {
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}