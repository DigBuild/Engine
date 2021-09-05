using System.Collections.Generic;

namespace DigBuild.Engine.Worldgen
{
    /// <summary>
    /// A basic type-aware dictionary of worldgen attributes.
    /// </summary>
    public sealed class WorldgenAttributeDictionary
    {
        private readonly Dictionary<IWorldgenAttribute, object> _dictionary = new();

        /// <summary>
        /// Gets the value of a worldgen attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public T Get<T>(WorldgenAttribute<T> attribute) where T : notnull
        {
            return (T) _dictionary[attribute];
        }

        /// <summary>
        /// Sets the value of a worldgen attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <param name="value">The value</param>
        public void Set<T>(WorldgenAttribute<T> attribute, T value) where T : notnull
        {
            _dictionary[attribute] = value;
        }

        /// <summary>
        /// Copies all the values from another dictionary.
        /// </summary>
        /// <param name="dictionary">The other dictionary</param>
        public void CopyFrom(WorldgenAttributeDictionary dictionary)
        {
            foreach (var (attribute, value) in dictionary._dictionary)
                _dictionary[attribute] = value;
        }

        /// <summary>
        /// Clears all the values in the dictionary.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}