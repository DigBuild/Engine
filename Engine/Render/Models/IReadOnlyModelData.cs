namespace DigBuild.Engine.Render.Models
{
    /// <summary>
    /// A read-only view of model data.
    /// </summary>
    public interface IReadOnlyModelData
    {
        /// <summary>
        /// Gets the value for a specific type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The value</returns>
        T? Get<T>() where T : notnull;
    }
}