using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render.New
{
    public interface IReadOnlyTextureSet
    {
        TextureSampler DefaultSampler { get; }

        Texture Get(RenderTexture texture);
    }
}