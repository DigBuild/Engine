namespace DigBuild.Engine.Render
{
    public interface IItemModel
    {
        void AddGeometry(GeometryBufferSet buffers);

        bool HasDynamicGeometry => false;
        void AddDynamicGeometry(GeometryBufferSet buffers) { }
    }
}