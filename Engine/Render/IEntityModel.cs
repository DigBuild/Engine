namespace DigBuild.Engine.Render
{
    public interface IEntityModel
    {
        void AddGeometry(GeometryBufferSet buffers, IReadOnlyModelData data, float partialTick);
    }
}