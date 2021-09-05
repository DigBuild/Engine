using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// A key-value read-only dictionary where they keys are resource names and the values are non-nullable types.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public sealed class Registry<T> : IReadOnlyDictionary<ResourceName, T> where T : notnull
    {
        private readonly IReadOnlyDictionary<ResourceName, T> _entries;
        private readonly IReadOnlyDictionary<T, ResourceName> _names;

        /// <summary>
        /// The name of the registry.
        /// </summary>
        public ResourceName Name { get; }

        internal Registry(ResourceName name, RegistryBuilder<T> builder)
        {
            Name = name;
            _entries = builder.Entries.ToImmutableDictionary();
            _names = builder.Entries.Select(pair => KeyValuePair.Create(pair.Value, pair.Key)).ToImmutableDictionary();
        }

        public int Count => _entries.Count;
        public IEnumerable<ResourceName> Keys => _entries.Keys;
        public IEnumerable<T> Values => _entries.Values;

        /// <summary>
        /// Checks whether a value with the given name is registered or not.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>Whether it is registered or not</returns>
        public bool Contains(ResourceName name)
        {
            return _entries.ContainsKey(name);
        }
        public bool Contains(string domain, string path)
        {
            return Contains(new ResourceName(domain, path));
        }

        /// <summary>
        /// Gets the value associated with a name, or null if missing.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The value, or null if missing</returns>
        public T? GetOrNull(ResourceName name)
        {
            _entries.TryGetValue(name, out var value);
            return value;
        }
        /// <summary>
        /// Gets the value associated with a name, or null if missing.
        /// </summary>
        /// <param name="domain">The domain</param>
        /// <param name="path">The path</param>
        /// <returns>The value, or null if missing</returns>
        public T? GetOrNull(string domain, string path)
        {
            return GetOrNull(new ResourceName(domain, path));
        }
        
        /// <summary>
        /// Tries to get the value associated with a name.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value, or null if missing</param>
        /// <returns>Whether the value was found or not</returns>
        public bool TryGet(ResourceName name, [MaybeNullWhen(false)] out T value)
        {
            return _entries.TryGetValue(name, out value);
        }
        /// <summary>
        /// Tries to get the value associated with a name.
        /// </summary>
        /// <param name="domain">The domain</param>
        /// <param name="path">The path</param>
        /// <param name="value">The value, or null if missing</param>
        /// <returns>Whether the value was found or not</returns>
        public bool TryGet(string domain, string path, [MaybeNullWhen(false)] out T value)
        {
            return TryGet(new ResourceName(domain, path), out value);
        }

        /// <summary>
        /// Gets the name for a value, or null if missing.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The name, or null if missing</returns>
        public ResourceName? GetNameOrNull(T value)
        {
            _names.TryGetValue(value, out var name);
            return name;
        }
        /// <summary>
        /// Tries to get the name for a value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="name">The name, or null if missing</param>
        /// <returns>Whether the name was found or not</returns>
        public bool TryGetName(T value, [MaybeNullWhen(false)] out ResourceName name)
        {
            return _names.TryGetValue(value, out name);
        }

        public IEnumerator<KeyValuePair<ResourceName, T>> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _entries).GetEnumerator();

        bool IReadOnlyDictionary<ResourceName, T>.ContainsKey(ResourceName key) => _entries.ContainsKey(key);
        bool IReadOnlyDictionary<ResourceName, T>.TryGetValue(ResourceName key, [MaybeNullWhen(false)] out T value) => _entries.TryGetValue(key, out value);
        T IReadOnlyDictionary<ResourceName, T>.this[ResourceName key] => _entries[key];
    }

    /// <summary>
    /// A registry builder.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public interface IRegistryBuilder<in T> where T : notnull
    {
        /// <summary>
        /// Adds a new registry entry.
        /// </summary>
        /// <typeparam name="T2">The actual value type</typeparam>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <returns>The value</returns>
        T2 Add<T2>(ResourceName name, T2 value) where T2 : T;
    }

    /// <summary>
    /// A registry builder.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public sealed class RegistryBuilder<T> : IRegistryBuilder<T> where T : notnull
    {
        internal readonly Dictionary<ResourceName, T> Entries = new();

        private readonly Predicate<ResourceName>? _nameValidator;
        private readonly Predicate<T>? _valueValidator;

        /// <summary>
        /// The registry name.
        /// </summary>
        public ResourceName Name { get; }

        internal RegistryBuilder(ResourceName name, Predicate<ResourceName>? nameValidator, Predicate<T>? valueValidator)
        {
            Name = name;
            _nameValidator = nameValidator;
            _valueValidator = valueValidator;
        }

        T2 IRegistryBuilder<T>.Add<T2>(ResourceName name, T2 value)
        {
            if (Entries.ContainsKey(name))
                throw new ArgumentException($"Name already in use: {name}", nameof(name));
            if (Entries.ContainsValue(value))
                throw new ArgumentException("Value already registered.", nameof(value));

            if (_nameValidator != null && _nameValidator(name))
                throw new ArgumentException($"Unsupported name: {name}", nameof(name));
            if (_valueValidator != null && _valueValidator(value))
                throw new ArgumentException($"Unsupported value: {value}", nameof(value));

            Entries[name] = value;
            return value;
        }
    }
}