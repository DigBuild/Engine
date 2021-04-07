using System;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    public sealed class SimpleSerdes<T> : ISerdes<T>
    {
        private readonly Action<Stream, T> _serialize;
        private readonly Func<Stream, T> _deserialize;

        public SimpleSerdes(Action<Stream, T> serialize, Func<Stream, T> deserialize)
        {
            _serialize = serialize;
            _deserialize = deserialize;
        }

        public void Serialize(Stream stream, T obj)
        {
            _serialize(stream, obj);
        }

        public T Deserialize(Stream stream)
        {
            return _deserialize(stream);
        }
    }
}