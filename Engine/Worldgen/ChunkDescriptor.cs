namespace DigBuild.Engine.Worldgen
{
    /// <summary>
    /// A chunk descriptor composed of an attribute dictionary.
    /// </summary>
    public sealed class ChunkDescriptor
    {
        private readonly WorldgenAttributeDictionary _attributes;

        internal ChunkDescriptor(WorldgenAttributeDictionary attributes)
        {
            _attributes = attributes;
        }

        /// <summary>
        /// Gets the value for a certain attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public T Get<T>(WorldgenAttribute<T> attribute)
            where T : notnull
        {
            return _attributes.Get(attribute);
        }
    }
}