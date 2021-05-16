using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    public interface IReadOnlyTextureSet
    {
        TextureSampler DefaultSampler { get; }

        Texture Get(RenderTexture texture);
    }
}