using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A custom shader resource.
    /// </summary>
    public sealed class Shader : ICustomResource
    {
        public ResourceName Name { get; }
        /// <summary>
        /// The backing resource.
        /// </summary>
        public IResource Resource { get; }

        private Shader(ResourceName name, IResource resource)
        {
            Name = name;
            Resource = resource;
        }

        public static Shader? Load(ResourceManager manager, ResourceName name)
        {
            var actualResourceName = new ResourceName(name.Domain, $"shaders/{name.Path}.spv");
            if (manager.TryGetResource(actualResourceName, out var actualResource))
                return new Shader(name, actualResource);
            return null;
        }
    }
}