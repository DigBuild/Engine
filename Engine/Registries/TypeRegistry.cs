using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    public interface ITypeRegistry<T, TValue> : IReadOnlyDictionary<Type, TValue>// : IReadOnlySet<Type>
    {
    }

    public sealed class TypeRegistry<T, TValue> : ITypeRegistry<T, TValue>
    {
        private readonly IReadOnlyDictionary<Type, TValue> _types;

        public ResourceName Name { get; }

        internal TypeRegistry(ResourceName name, TypeRegistryBuilder<T, TValue> builder)
        {
            Name = name;
            _types = builder.Types.ToImmutableDictionary();
        }

        public int Count => _types.Count;
        public IEnumerable<Type> Keys => _types.Keys;
        public IEnumerable<TValue> Values => _types.Values;

        public bool ContainsKey(Type key) => _types.ContainsKey(key);

        public bool TryGetValue(Type key, [MaybeNullWhen(false)] out TValue value) => _types.TryGetValue(key, out value);

        public TValue this[Type key] => _types[key];

        public IEnumerator<KeyValuePair<Type, TValue>> GetEnumerator() => _types.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _types).GetEnumerator();
    }

    public interface ITypeRegistryBuilder<T, in TValue>
    {
        T2 Add<T2>(Type type, T2 value) where T2 : TValue;
    }

    public sealed class TypeRegistryBuilder<T, TValue> : ITypeRegistryBuilder<T, TValue>
    {
        internal readonly Dictionary<Type, TValue> Types = new();
        
        private readonly Predicate<Type>? _typeValidator;
        private readonly Predicate<TValue>? _valueValidator;

        public ResourceName Name { get; }

        internal TypeRegistryBuilder(ResourceName name, Predicate<Type>? typeValidator, Predicate<TValue>? valueValidator)
        {
            Name = name;
            _typeValidator = typeValidator;
            _valueValidator = valueValidator;
        }
        
        T2 ITypeRegistryBuilder<T, TValue>.Add<T2>(Type type, T2 value)
        {
            if (Types.ContainsKey(type))
                throw new ArgumentException($"Type already registered: {type.FullName}", nameof(type));
            if (Types.ContainsValue(value))
                throw new ArgumentException($"Value already registered.", nameof(value));
            
            if (_typeValidator != null && _typeValidator(type))
                throw new ArgumentException($"Unsupported type: {type.FullName}", nameof(type));
            if (_valueValidator != null && _valueValidator(value))
                throw new ArgumentException($"Unsupported value: {value}", nameof(value));

            Types[type] = value;
            return value;
        }
    }
}