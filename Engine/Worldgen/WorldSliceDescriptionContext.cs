using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worldgen
{
    public sealed class WorldSliceDescriptionContext
    {
        public delegate WorldSliceDescriptor DescribeNeighborDelegate(WorldSlicePos pos);

        private readonly WorldgenAttributeDictionary _attributes = new();
        private readonly WorldgenAttributeDictionary _newAttributes = new();

        public WorldSlicePos Position { get; }
        public long Seed { get; }

        public DescribeNeighborDelegate NeighborDescriptor { private get; set; } = null!;

        internal WorldSliceDescriptionContext(WorldSlicePos position, long seed)
        {
            Position = position;
            Seed = seed;
        }

        public TStorage Get<TStorage>(WorldgenAttribute<TStorage> attribute, WorldSliceOffset offset = default)
            where TStorage : notnull
        {
            if (!offset.Equals(default(WorldSliceOffset)))
                return NeighborDescriptor(Position + offset).Get(attribute);
            
            return _attributes.Get(attribute);
        }

        public void Submit<TStorage>(WorldgenAttribute<TStorage> attribute, TStorage value)
            where TStorage : notnull
        {
            _newAttributes.Set(attribute, value);
        }

        internal void Next()
        {
            _attributes.SetAll(_newAttributes);
            _newAttributes.Clear();
        }

        internal WorldSliceDescriptor CreateDescriptor()
        {
            return new(Position, _attributes);
        }
    }
}