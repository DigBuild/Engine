using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worldgen
{
    public sealed class ChunkDescriptionContext
    {
        public delegate ChunkDescriptor DescribeNeighborDelegate(ChunkPos pos);

        private readonly WorldgenAttributeDictionary _attributes = new();
        private readonly WorldgenAttributeDictionary _newAttributes = new();

        public ChunkPos Position { get; }
        public long Seed { get; }

        internal DescribeNeighborDelegate NeighborDescriptor { private get; set; } = null!;

        internal ChunkDescriptionContext(ChunkPos position, long seed)
        {
            Position = position;
            Seed = seed;
        }

        public TStorage Get<TStorage>(WorldgenAttribute<TStorage> attribute, ChunkOffset offset = default)
            where TStorage : notnull
        {
            if (!offset.Equals(default(ChunkOffset)))
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

        internal ChunkDescriptor CreateDescriptor()
        {
            return new(Position, _attributes);
        }
    }
}