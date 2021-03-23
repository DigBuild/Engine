using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DigBuild.Engine.Registries
{
    public interface IExtendedTypeRegistry<T>// : IReadOnlySet<Type>
    {
    }

    public sealed class ExtendedTypeRegistry<T, TValue> : IReadOnlyDictionary<Type, TValue>, IExtendedTypeRegistry<T>
    {
        private readonly IReadOnlyDictionary<Type, TValue> _types;

        internal ExtendedTypeRegistry(ExtendedTypeRegistryBuilder<T, TValue> builder)
        {
            _types = builder.Types.ToImmutableDictionary();
        }

        public int Count => _types.Count;
        public IEnumerable<Type> Keys => _types.Keys;
        public IEnumerable<TValue> Values => _types.Values;

        public bool ContainsKey(Type key) => _types.ContainsKey(key);

        public bool TryGetValue(Type key, out TValue value) => _types.TryGetValue(key, out value);

        public TValue this[Type key] => _types[key];

        public IEnumerator<KeyValuePair<Type, TValue>> GetEnumerator() => _types.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _types).GetEnumerator();
    }

    public interface IExtendedTypeRegistryBuilder<T, in TValue>
    {
        void Add(Type type, TValue value);
    }

    public sealed class ExtendedTypeRegistryBuilder<T, TValue> : IExtendedTypeRegistryBuilder<T, TValue>
    {
        internal readonly Dictionary<Type, TValue> Types = new();
        
        private readonly Predicate<Type> _typeValidator;

        internal ExtendedTypeRegistryBuilder(Predicate<Type> typeValidator)
        {
            _typeValidator = typeValidator;
        }

        void IExtendedTypeRegistryBuilder<T, TValue>.Add(Type type, TValue value)
        {
            if (!_typeValidator(type))
                throw new ArgumentException($"Incompatible type: {type.FullName}", nameof(type));
            if (Types.ContainsKey(type))
                throw new ArgumentException($"Type was already registered: {type.FullName}", nameof(type));
            Types[type] = value;
        }
    }
}