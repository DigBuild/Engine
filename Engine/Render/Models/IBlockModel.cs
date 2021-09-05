using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render.Models
{
    /// <summary>
    /// A block model.
    /// </summary>
    public interface IBlockModel
    {
        /// <summary>
        /// Adds the block's geometry to the buffer.
        /// </summary>
        /// <param name="buffer">The geometry buffer</param>
        /// <param name="data">The model data</param>
        /// <param name="visibleFaces">The visible faces</param>
        void AddGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, DirectionFlags visibleFaces);

        /// <summary>
        /// Whether this model contains dynamic render state.
        /// </summary>
        bool HasDynamicGeometry { get; }

        /// <summary>
        /// Adds the block's dynamic geometry to the buffer.
        /// </summary>
        /// <param name="buffer">The geometry buffer</param>
        /// <param name="data">The model data</param>
        /// <param name="visibleFaces">The visible faces</param>
        /// <param name="partialTick">The tick delta</param>
        void AddDynamicGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, DirectionFlags visibleFaces, float partialTick);
    }
}