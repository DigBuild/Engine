using System.Collections;
using System.Collections.Generic;

namespace DigBuild.Engine.Collections
{
    public sealed class Grid<T> : IReadOnlyCollection<T>
    {
        internal readonly T[,] Values;
        
        public int Size => Values.GetLength(0);
        public int Count => Values.Length;

        public Grid(T[,] values)
        {
            Values = values;
        }

        public T this[int x, int z] => Values[x, z];
        
        public IEnumerator<T> GetEnumerator()
        {
            var size = Values.GetLength(0);
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                yield return Values[i, j];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public GridBuilder<T> ToBuilder()
        {
            return new(this);
        }

        public static GridBuilder<T> Builder(uint size, T defaultValue = default!)
        {
            return new(size, defaultValue);
        }
    }

    public sealed class GridBuilder<T>
    {
        private readonly T[,] _values;

        public int Size => _values.GetLength(0);
        public int Count => _values.Length;

        internal GridBuilder(uint size, T defaultValue)
        {
            _values = new T[size, size];
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                _values[i, j] = defaultValue;
        }

        internal GridBuilder(Grid<T> other)
        {                
            _values = (T[,]) other.Values.Clone();
        }

        public T this[int x, int z]
        {
            get => _values[x, z];
            set => _values[x, z] = value;
        }

        public Grid<T> Build()
        {
            return new((T[,]) _values.Clone());
        }
    }

    public static class GridExtensions
    {
        public static void Add(this GridBuilder<float> self, Grid<float> other)
        {
            for (var i = 0; i < self.Size; i++)
            for (var j = 0; j < self.Size; j++)
                self[i, j] += other[i, j];
        }
    }
}