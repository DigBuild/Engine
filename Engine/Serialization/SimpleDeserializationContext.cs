using System;
using System.Collections;
using System.Collections.Generic;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A basic dictionary-based deserialization context.
    /// </summary>
    public class SimpleDeserializationContext : IDeserializationContext, IEnumerable<object>
    {
        private readonly IDeserializationContext? _parent;
        private readonly Dictionary<Type, object> _dictionary = new();

        public SimpleDeserializationContext(IDeserializationContext? parent = null)
        {
            _parent = parent;
        }

        /// <summary>
        /// Adds a new value to the context and overrides any previous one for the same type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value</param>
        public void Add<T>(T value)
            where T : notnull
        {
            _dictionary[typeof(T)] = value;
        }

        public T Get<T>()
        {
            if (_dictionary.TryGetValue(typeof(T), out var value))
                return (T)value;
            return (_parent != null ? _parent.Get<T>() : default)!;
        }

        public IEnumerator<object> GetEnumerator()
        {
            yield break;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}