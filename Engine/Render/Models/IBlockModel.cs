using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render.Models
{
    public interface IBlockModel
    {
        void AddGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, DirectionFlags visibleFaces);

        bool HasDynamicGeometry { get; }
        void AddDynamicGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, DirectionFlags visibleFaces, float partialTick);
    }
}