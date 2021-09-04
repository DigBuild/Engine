using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Collections
{
    /// <summary>
    /// A read-only octree.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface IReadOnlyOctree<T> : IEnumerable<KeyValuePair<Vector3I, T>>
    {
        /// <summary>
        /// A value at a given position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The value</returns>
        T this[int x, int y, int z] { get; }
    }
}