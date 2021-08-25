using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    public sealed class Registry<T> : IReadOnlyDictionary<ResourceName, T> where T : notnull
    {
        private readonly IReadOnlyDictionary<ResourceName, T> _entries;
        private readonly IReadOnlyDictionary<T, ResourceName> _names;

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

        public bool Contains(ResourceName name)
        {
            return _entries.ContainsKey(name);
        }
        public bool Contains(string domain, string path)
        {
            return Contains(new ResourceName(domain, path));
        }

        public T? GetOrNull(ResourceName name)
        {
            _entries.TryGetValue(name, out var value);
            return value;
        }
        public T? GetOrNull(string domain, string path)
        {
            return GetOrNull(new ResourceName(domain, path));
        }

        public bool TryGet(ResourceName name, [MaybeNullWhen(false)] out T value)
        {
            return _entries.TryGetValue(name, out value);
        }
        public bool TryGet(string domain, string path, [MaybeNullWhen(false)] out T value)
        {
            return TryGet(new ResourceName(domain, path), out value);
        }

        public ResourceName? GetNameOrNull(T value)
        {
            _names.TryGetValue(value, out var name);
            return name;
        }
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

    public interface IRegistryBuilder<in T> where T : notnull
    {
        T2 Add<T2>(ResourceName name, T2 value) where T2 : T;
    }

    public sealed class RegistryBuilder<T> : IRegistryBuilder<T> where T : notnull
    {
        internal readonly Dictionary<ResourceName, T> Entries = new();

        private readonly Predicate<ResourceName>? _nameValidator;
        private readonly Predicate<T>? _valueValidator;

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