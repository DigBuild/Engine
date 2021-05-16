namespace DigBuild.Engine.Render.Models
{
    public interface IEntityModel
    {
        void AddGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, float partialTick);
    }
}