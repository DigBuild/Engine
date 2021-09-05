using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A bit stream object serializer-deserializer.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface ISerdes<T> : IDeserializer<T>
    {
        /// <summary>
        /// Serializes the given object into the bit stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="obj">The object</param>
        void Serialize(Stream stream, T obj);
    }

    /// <summary>
    /// Extensions for serdes.
    /// </summary>
    public static class SerdesExtensions
    {
        /// <summary>
        /// Casts a serdes' type to a supertype.
        /// </summary>
        /// <typeparam name="T">The serdes type</typeparam>
        /// <typeparam name="T2">The supertype</typeparam>
        /// <param name="serdes">The serdes</param>
        /// <returns>A new serdes</returns>
        public static ISerdes<T2> UncheckedSuperCast<T, T2>(this ISerdes<T> serdes)
            where T : T2
            where T2 : class
        {
            return new SimpleSerdes<T2>(
                (stream, obj) => serdes.Serialize(stream, (T) obj),
                (stream, ctx) => serdes.Deserialize(stream, ctx)!
            );
        }
    }
}