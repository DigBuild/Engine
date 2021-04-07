using System;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    public sealed class SimpleDeserializer<T> : IDeserializer<T>
    {
        private readonly Func<Stream, T> _deserialize;

        public SimpleDeserializer(Func<Stream, T> deserialize)
        {
            _deserialize = deserialize;
        }
        
        public T Deserialize(Stream stream)
        {
            return _deserialize(stream);
        }
    }
}