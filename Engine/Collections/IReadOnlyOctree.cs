using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Collections
{
    public interface IReadOnlyOctree<T> : IEnumerable<KeyValuePair<Vector3I, T>>
    {
        // IEnumerable<T> Values { get; }

        T this[int x, int y, int z] { get; }
    }
}