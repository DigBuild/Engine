using System.Numerics;

namespace DigBuild.Engine.Textures
{
    public interface ISprite
    {
        public Vector2 UV { get; }
        public Vector2 Size { get; }

        public Vector2 GetInterpolatedUV(float partialU, float partialV)
        {
            return UV + Size * new Vector2(partialU, partialV);
        }
    }
}