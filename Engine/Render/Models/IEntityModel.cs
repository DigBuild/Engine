namespace DigBuild.Engine.Render.Models
{
    /// <summary>
    /// An entity model.
    /// </summary>
    public interface IEntityModel
    {
        /// <summary>
        /// Adds the entity's geometry to the geometry buffer.
        /// </summary>
        /// <param name="buffer">The geometry buffer</param>
        /// <param name="data">The model data</param>
        /// <param name="partialTick">The tick delta</param>
        void AddGeometry(IGeometryBuffer buffer, IReadOnlyModelData data, float partialTick);
    }
}