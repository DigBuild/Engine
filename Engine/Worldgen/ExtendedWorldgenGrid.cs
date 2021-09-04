using System.Collections.Generic;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worldgen
{
    /// <summary>
    /// A grid that can query external values for a wordlgen attribute.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public sealed class ExtendedWorldgenGrid<T>
    {
        private readonly Dictionary<ChunkOffset, Grid<T>> _cache = new();

        private readonly ChunkDescriptionContext _context;
        private readonly WorldgenAttribute<Grid<T>> _attribute;

        internal ExtendedWorldgenGrid(ChunkDescriptionContext context, WorldgenAttribute<Grid<T>> attribute)
        {
            _context = context;
            _attribute = attribute;
        }

        public ExtendedWorldgenGrid(Grid<T> originGrid, ExtendedWorldgenGrid<T> parent)
        {
            _context = parent._context;
            _attribute = parent._attribute;
            foreach (var (offset, grid) in parent._cache)
                _cache.Add(offset, grid);
            _cache[default] = originGrid;
        }
        
        /// <summary>
        /// A grid element.
        /// </summary>
        /// <param name="i">The first coordinate</param>
        /// <param name="j">The second coordinate</param>
        /// <returns>The element</returns>
        public T this[int i, int j]
        {
            get {
                var off = new ChunkOffset(i >> 4, j >> 4);
                if (!_cache.TryGetValue(off, out var data))
                    _cache[off] = data = _context.Get(_attribute, off);
                return data[i & 15, j & 15];
            }
        }
        
        /// <summary>
        /// Creates a grid builder based on the contents of this grid.
        /// </summary>
        /// <returns>The builder</returns>
        public GridBuilder<T> ToBuilder()
        {
            if (!_cache.TryGetValue(default, out var data))
                _cache[default] = data = _context.Get(_attribute);
            return data.ToBuilder();
        }
    }
}