using System.Numerics;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A geometry buffer.
    /// </summary>
    public interface IGeometryBuffer
    {
        /// <summary>
        /// A transform to apply to incoming geometry.
        /// </summary>
        Matrix4x4 Transform { get; set; } 
        /// <summary>
        /// Whether to apply the transform to normals too.
        /// </summary>
        bool TransformNormal { get; set; } 
 
        /// <summary>
        /// Gets the vertex consumer for a given render layer.
        /// </summary>
        /// <typeparam name="TVertex">The vertex type</typeparam>
        /// <param name="layer">The layer</param>
        /// <returns>The consumer</returns>
        IVertexConsumer<TVertex> Get<TVertex>(IRenderLayer<TVertex> layer)
            where TVertex : unmanaged; 
    }
}