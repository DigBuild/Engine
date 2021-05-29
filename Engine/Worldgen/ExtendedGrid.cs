using System.Collections.Generic;
using DigBuild.Engine.Collections;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worldgen
{
    public sealed class ExtendedGrid<T>
    {
        private readonly Dictionary<WorldSliceOffset, Grid<T>> _cache = new();

        private readonly WorldSliceDescriptionContext _context;
        private readonly WorldgenAttribute<Grid<T>> _attribute;

        internal ExtendedGrid(WorldSliceDescriptionContext context, WorldgenAttribute<Grid<T>> attribute)
        {
            _context = context;
            _attribute = attribute;
        }

        public T this[int x, int z]
        {
            get {
                var off = new WorldSliceOffset(x >> 4, z >> 4);
                if (!_cache!.TryGetValue(off, out var data))
                    _cache[off] = data = _context.Get(_attribute, off);
                return data[x & 15, z & 15];
            }
        }
        public GridBuilder<T> ToBuilder()
        {
            if (!_cache!.TryGetValue(default, out var data))
                _cache[default] = data = _context.Get(_attribute);
            return data.ToBuilder();
        }
    }
}