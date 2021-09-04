namespace DigBuild.Engine.Collections
{
    /// <summary>
    /// An octree.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface IOctree<T> : IReadOnlyOctree<T>
    {
        /// <summary>
        /// A value at a given position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The value</returns>
        new T this[int x, int y, int z] { get; set; }
    }
}