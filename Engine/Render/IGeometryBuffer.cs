using System.Numerics;

namespace DigBuild.Engine.Render
{
    public interface IGeometryBuffer
    {
        Matrix4x4 Transform { get; set; } 
        bool TransformNormal { get; set; } 
 
        IVertexConsumer<TVertex> Get<TVertex>(IRenderLayer<TVertex> layer)
            where TVertex : unmanaged; 
    }
}