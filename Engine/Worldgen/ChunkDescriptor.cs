using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worldgen
{
    public sealed class ChunkDescriptor
    {
        private readonly WorldgenAttributeDictionary _attributes;

        public ChunkPos Position { get; }

        internal ChunkDescriptor(ChunkPos position, WorldgenAttributeDictionary attributes)
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