namespace DigBuild.Engine.Render
{
    public interface IItemModel
    {
        void AddGeometry(ItemModelTransform transform, GeometryBufferSet buffers);

        bool HasDynamicGeometry => false;
        void AddDynamicGeometry(ItemModelTransform transform, GeometryBufferSet buffers) { }
    }

    public enum ItemModelTransform
    {
        None,
        Inventory
    }
}