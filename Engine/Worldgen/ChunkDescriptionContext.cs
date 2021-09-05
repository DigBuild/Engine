using System;
using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worldgen
{
    /// <summary>
    /// A chunk description context.
    /// </summary>
    public sealed class ChunkDescriptionContext
    {
        public delegate ChunkDescriptor DescribeNeighborDelegate(ChunkPos pos);

        private readonly WorldgenAttributeDictionary _attributes = new();
        private readonly WorldgenAttributeDictionary _newAttributes = new();

        /// <summary>
        /// The chunk position.
        /// </summary>
        public ChunkPos Position { get; }

        /// <summary>
        /// The seed used for generation.
        /// </summary>
        public long Seed { get; }

        internal DescribeNeighborDelegate NeighborDescriptor { private get; set; } = null!;

        internal ChunkDescriptionContext(ChunkPos position, long seed)
        {
            Position = position;
            Seed = seed;
        }

        /// <summary>
        /// Gets the value of an attribute at the given offset.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <param name="offset">The offset</param>
        /// <returns>The value</returns>
        public T Get<T>(WorldgenAttribute<T> attribute, ChunkOffset offset = default)
            where T : notnull
        {
            if (!offset.Equals(default(ChunkOffset)))
                return NeighborDescriptor(Position + offset).Get(attribute);
            
            return _attributes.Get(attribute);
        }

        /// <summary>
        /// Submits a new value for the given attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <param name="value">The value</param>
        public void Submit<T>(WorldgenAttribute<T> attribute, T value)
            where T : notnull
        {
            _newAttributes.Set(attribute, value);
        }

        internal void Next()
        {
            _attributes.CopyFrom(_newAttributes);
            _newAttributes.Clear();
        }

        internal ChunkDescriptor CreateDescriptor()
        {
            return new ChunkDescriptor(_attributes);
        }
    }
}