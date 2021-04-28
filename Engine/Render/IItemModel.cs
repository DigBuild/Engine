namespace DigBuild.Engine.Render
{
    public interface IItemModel
    {
        void AddGeometry(GeometryBufferSet buffers, IReadOnlyModelData data, ItemModelTransform transform, float partialTick);
    }

    public enum ItemModelTransform
    {
        None,
        Inventory
    }
}