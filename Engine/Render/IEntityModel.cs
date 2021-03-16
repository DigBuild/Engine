using DigBuild.Engine.Entities;

namespace DigBuild.Engine.Render
{
    public interface IEntityModel
    {
        void AddGeometry(EntityInstance entity, GeometryBufferSet buffers);

        bool HasDynamicGeometry => false;
        void AddDynamicGeometry(EntityInstance entity, GeometryBufferSet buffers) { }
    }
}