namespace DigBuild.Engine.Render.Models
{
    /// <summary>
    /// An item model.
    /// </summary>
    public interface IItemModel
    {
        /// <summary>
        /// Adds the item's geometry to the geometry buffer.
        /// </summary>
        /// <param name="buffer">The geometry buffer</param>
        /// <param name="data">The model data</param>
        /// <param name="transform"></param>
        /// <param name="partialTick">The tick delta</param>
        void AddGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, ItemModelTransform transform, float partialTick);
    }
}