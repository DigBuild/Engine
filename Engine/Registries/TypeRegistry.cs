using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// A key-value read-only dictionary where they keys are C# types.
    /// </summary>
    /// <typeparam name="T">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public sealed class TypeRegistry<T, TValue> : IReadOnlyDictionary<Type, TValue>
    {
        private readonly IReadOnlyDictionary<Type, TValue> _types;

        /// <summary>
        /// The name of the registry.
        /// </summary>
        public ResourceName Name { get; }

        internal TypeRegistry(ResourceName name, TypeRegistryBuilder<T, TValue> builder)
        {
            Name = name;
            _types = builder.Types.ToImmutableDictionary();
        }

        public int Count => _types.Count;
        public IEnumerable<Type> Keys => _types.Keys;
        public IEnumerable<TValue> Values => _types.Values;
        
        /// <summary>
        /// Checks whether a value of the given type is registered or not.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>Whether it is registered or not</returns>
        public bool ContainsKey(Type type) => _types.ContainsKey(type);

        /// <summary>
        /// Tries to get the value associated with a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="value">The value, or null if missing</param>
        /// <returns>Whether the value was found or not</returns>
        public bool TryGetValue(Type type, [MaybeNullWhen(false)] out TValue value) => _types.TryGetValue(type, out value);

        /// <summary>
        /// The value for a given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The value, or null if missing</returns>
        public TValue this[Type type] => _types[type];

        public IEnumerator<KeyValuePair<Type, TValue>> GetEnumerator() => _types.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _types).GetEnumerator();
    }

    /// <summary>
    /// A type registry builder.
    /// </summary>
    /// <typeparam name="T">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface ITypeRegistryBuilder<T, in TValue>
    {
        T2 Add<T2>(Type type, T2 value) where T2 : TValue;
    }
    
    /// <summary>
    /// A type registry builder.
    /// </summary>
    /// <typeparam name="T">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public sealed class TypeRegistryBuilder<T, TValue> : ITypeRegistryBuilder<T, TValue>
    {
        internal readonly Dictionary<Type, TValue> Types = new();
        
        private readonly Predicate<Type>? _typeValidator;
        private readonly Predicate<TValue>? _valueValidator;

        /// <summary>
        /// The registry name.
        /// </summary>
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