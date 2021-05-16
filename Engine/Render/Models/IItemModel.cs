namespace DigBuild.Engine.Render.Models
{
    public interface IItemModel
    {
        void AddGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, ItemModelTransform transform, float partialTick);
    }
}