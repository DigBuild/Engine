﻿using System;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    public sealed class SimpleDeserializer<T> : IDeserializer<T>
    {
        private readonly Func<Stream, IDeserializationContext, T> _deserialize;

        public SimpleDeserializer(Func<Stream, IDeserializationContext, T> deserialize)
        {
            _deserialize = deserialize;
        }
        
        public T Deserialize(Stream stream, IDeserializationContext context)
        {
            return _deserialize(stream, context);
        }
    }
}