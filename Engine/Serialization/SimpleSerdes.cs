using System;
using System.Collections.Generic;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A simple serdes that wraps a serialization and deserialization delegate.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public sealed class SimpleSerdes<T> : ISerdes<T>
    {
        private readonly Action<Stream, T> _serialize;
        private readonly Func<Stream, IDeserializationContext, T> _deserialize;

        public SimpleSerdes(Action<Stream, T> serialize, Func<Stream, IDeserializationContext, T> deserialize)
        {
            _serialize = serialize;
            _deserialize = deserialize;
        }

        public void Serialize(Stream stream, T obj)
        {
            _serialize(stream, obj);
        }

        public T Deserialize(Stream stream, IDeserializationContext context)
        {
            return _deserialize(stream, context);
        }
    }
}