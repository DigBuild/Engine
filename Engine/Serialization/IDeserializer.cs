using System;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A bit stream object deserializer.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface IDeserializer<out T>
    {
        public delegate T Delegate(Stream stream, IDeserializationContext context);

        /// <summary>
        /// Creates a new object from the data in the bit stream and the context.
        /// </summary>
        /// <param name="stream">The bit stream</param>
        /// <param name="context">The context</param>
        /// <returns>The new object</returns>
        T Deserialize(Stream stream, IDeserializationContext context);
    }

    /// <summary>
    /// Extensions for deserializers.
    /// </summary>
    public static class DeserializerExtensions
    {
        /// <summary>
        /// Applies an adapter to a deserializer's output.
        /// </summary>
        /// <typeparam name="T">The deserializer type</typeparam>
        /// <typeparam name="T2">The new type</typeparam>
        /// <param name="deserializer">The deserializer</param>
        /// <param name="adapter">The adapter</param>
        /// <returns>A new deserializer</returns>
        public static IDeserializer<T2> WithAdapter<T, T2>(this IDeserializer<T> deserializer, Func<T, T2> adapter)
        {
            return new SimpleDeserializer<T2>(
                (stream, ctx) => adapter(deserializer.Deserialize(stream, ctx))
            );
        }
    }
}