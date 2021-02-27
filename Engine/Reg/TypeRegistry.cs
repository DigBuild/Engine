using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DigBuildEngine.Reg
{
    public sealed class TypeRegistry<T> : IReadOnlySet<Type>
    {
        private readonly IReadOnlySet<Type> _types;

        internal TypeRegistry(TypeRegistryBuilder<T> builder)
        {
            _types = builder.Types.ToImmutableHashSet();
        }

        public int Count => _types.Count;

        public bool Contains(Type item) => _types.Contains(item);

        public IEnumerator<Type> GetEnumerator() => _types.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _types).GetEnumerator();

        bool IReadOnlySet<Type>.IsProperSubsetOf(IEnumerable<Type> other) => _types.IsProperSubsetOf(other);
        bool IReadOnlySet<Type>.IsProperSupersetOf(IEnumerable<Type> other) => _types.IsProperSupersetOf(other);
        bool IReadOnlySet<Type>.IsSubsetOf(IEnumerable<Type> other) => _types.IsSubsetOf(other);
        bool IReadOnlySet<Type>.IsSupersetOf(IEnumerable<Type> other) => _types.IsSupersetOf(other);
        bool IReadOnlySet<Type>.Overlaps(IEnumerable<Type> other) => _types.Overlaps(other);
        bool IReadOnlySet<Type>.SetEquals(IEnumerable<Type> other) => _types.SetEquals(other);
    }

    public sealed class TypeRegistryBuilder<T>
    {
        internal readonly HashSet<Type> Types = new();
        
        private readonly Predicate<Type> _typeValidator;

        internal TypeRegistryBuilder(Predicate<Type> typeValidator)
        {
            _typeValidator = typeValidator;
        }

        public void Add(Type type)
        {
            if (!_typeValidator(type))
                throw new ArgumentException($"Incompatible type: {type.FullName}", nameof(type));
            Types.Add(type);
        }
    }
}