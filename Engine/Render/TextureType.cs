using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A texture handle.
    /// </summary>
    public sealed class TextureType
    {
        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }
        
        public TextureType(ResourceName name)
        {
            Name = name;
        }

        public TextureType(string domain, string path) : this(new ResourceName(domain, path))
        {
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}