using System.Collections;
using System.Collections.Generic;

namespace DigBuild.Engine.Worldgen
{
    public sealed class ImmutableMap2D<T> : IReadOnlyCollection<T> where T : unmanaged
    {
        internal readonly T[,] Values;

        public int Count => Values.Length;

        public ImmutableMap2D(T[,] values)
        {
            Values = values;
        }

        public T this[int x, int z] => Values[x, z];
        
        public IEnumerator<T> GetEnumerator()
        {
            var size = Values.GetLength(0);
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    yield return Values[i, j];
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class ImmutableMap2DBuilder<T> where T : unmanaged
    {
        private readonly T[,] _values;

        public ImmutableMap2DBuilder(uint size, T defaultValue = default)
        {
            _values = new T[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    _values[i, j] = defaultValue;
                }
            }
        }

        public ImmutableMap2DBuilder(ImmutableMap2D<T> other)
        {
            _values = (other.Values.Clone() as T[,])!;
        }

        public T this[int x, int z]
        {
            get => _values[x, z];
            set => _values[x, z] = value;
        }

        public ImmutableMap2D<T> Build()
        {
            return new(_values);
        }
    }
}