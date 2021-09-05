namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A context for deserialization operations.
    /// </summary>
    public interface IDeserializationContext
    {
        /// <summary>
        /// Gets the value associated with a given type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The value, or null if missing</returns>
        T? Get<T>();
    }
}