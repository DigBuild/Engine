using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A collection of textures.
    /// </summary>
    public interface IReadOnlyTextureSet
    {
        /// <summary>
        /// A default general-purpose sampler.
        /// </summary>
        TextureSampler DefaultSampler { get; }

        /// <summary>
        /// Gets a texture.
        /// </summary>
        /// <param name="textureType">The texture type</param>
        /// <returns>The corresponding texture</returns>
        Texture Get(TextureType textureType);
    }
}