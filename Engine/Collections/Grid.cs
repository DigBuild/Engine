using System;
using System.Collections;
using System.Collections.Generic;

namespace DigBuild.Engine.Collections
{
    /// <summary>
    /// A two-dimensional grid of values.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public sealed class Grid<T> : IReadOnlyCollection<T>
    {
        internal readonly T[,] Values;
        
        /// <summary>
        /// The size of the grid in one direction.
        /// </summary>
        public int Size => Values.GetLength(0);
        /// <summary>
        /// The amount of elements in the grid.
        /// </summary>
        public int Count => Values.Length;

        public Grid(T[,] values)
        {
            Values = values;
        }

        /// <summary>
        /// A grid element.
        /// </summary>
        /// <param name="i">The first coordinate</param>
        /// <param name="j">The second coordinate</param>
        /// <returns>The element</returns>
        public T this[int i, int j] => Values[i, j];
        
        /// <summary>
        /// Gets an enumerator for all the grid elements.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var size = Values.GetLength(0);
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                yield return Values[i, j];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Creates a grid builder based on the contents of this grid.
        /// </summary>
        /// <returns>The builder</returns>
        public GridBuilder<T> ToBuilder()
        {
            return new GridBuilder<T>(this);
        }

        /// <summary>
        /// Creates a new grid builder of the given size and default value.
        /// </summary>
        /// <param name="size">The size</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The builder</returns>
        public static GridBuilder<T> Builder(uint size, T defaultValue = default!)
        {
            return new GridBuilder<T>(size, defaultValue);
        }
    }

    /// <summary>
    /// A grid builder.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public sealed class GridBuilder<T>
    {
        private readonly T[,] _values;
        
        /// <summary>
        /// The size of the grid in one direction.
        /// </summary>
        public int Size => _values.GetLength(0);
        /// <summary>
        /// The amount of elements in the grid.
        /// </summary>
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
        
        /// <summary>
        /// A grid element.
        /// </summary>
        /// <param name="i">The first coordinate</param>
        /// <param name="j">The second coordinate</param>
        /// <returns>The element</returns>
        public T this[int i, int j]
        {
            get => _values[i, j];
            set => _values[i, j] = value;
        }

        /// <summary>
        /// Creates a grid out of the populated data.
        /// </summary>
        /// <returns>The grid</returns>
        public Grid<T> Build()
        {
            return new Grid<T>((T[,]) _values.Clone());
        }
    }

    /// <summary>
    /// Grid utilities.
    /// </summary>
    public static class GridExtensions
    {
        /// <summary>
        /// Adds a float grid to a float grid builder.
        /// </summary>
        /// <param name="self">The builder</param>
        /// <param name="other">The grid</param>
        public static void Add(this GridBuilder<float> self, Grid<float> other)
        {
            for (var i = 0; i < self.Size; i++)
            for (var j = 0; j < self.Size; j++)
                self[i, j] += other[i, j];
        }
    }
}