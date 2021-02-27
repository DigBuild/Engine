using System.Collections.Generic;

namespace DigBuildEngine.Worldgen
{
    public sealed class WorldgenAttributeDictionary
    {
        private readonly Dictionary<IWorldgenAttribute, object> _dictionary = new();

        public TStorage Get<TStorage>(WorldgenAttribute<TStorage> attribute) where TStorage : notnull
        {
            return (TStorage) _dictionary[attribute];
        }

        public void Set<TStorage>(WorldgenAttribute<TStorage> attribute, TStorage value) where TStorage : notnull
        {
            _dictionary[attribute] = value;
        }

        public void SetAll(WorldgenAttributeDictionary dictionary)
        {
            foreach (var (attribute, value) in dictionary._dictionary)
                _dictionary[attribute] = value;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}