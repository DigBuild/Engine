using System;
using DigBuildEngine.Math;

namespace DigBuildEngine.Worldgen
{
    public sealed class WorldSliceDescriptor
    {
        private readonly WorldgenAttributeDictionary _attributes;

        public WorldSlicePos Position { get; }

        internal WorldSliceDescriptor(WorldSlicePos position, WorldgenAttributeDictionary attributes)
        {
            Position = position;
            _attributes = attributes;
        }

        public TStorage Get<TStorage>(WorldgenAttribute<TStorage> attribute)
            where TStorage : notnull
        {
            return _attributes.Get(attribute);
        }
    }
}