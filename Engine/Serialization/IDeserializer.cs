using System;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    public interface IDeserializer<out T>
    {
        public delegate T Delegate(Stream stream, IDeserializationContext context);

        T Deserialize(Stream stream, IDeserializationContext context);
    }

    public static class DeserializerExtensions
    {
        public static IDeserializer<T2> Casting<T, T2>(this IDeserializer<T> deserializer, Func<T, T2> cast)
        {
            return new SimpleDeserializer<T2>(
                (stream, ctx) => cast(deserializer.Deserialize(stream, ctx))
            );
        }
    }
}