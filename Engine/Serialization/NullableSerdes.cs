using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A serdes wrapper that applies nullability checks.
    /// </summary>
    /// <typeparam name="T">The non-null type</typeparam>
    public sealed class NullableSerdes<T> : ISerdes<T?> where T : class
    {
        private readonly ISerdes<T> _serdes;

        internal NullableSerdes(ISerdes<T> serdes)
        {
            _serdes = serdes;
        }

        public void Serialize(Stream stream, T? obj)
        {
            stream.WriteByte((byte) (obj != null ? 1 : 0));
            if (obj != null)
                _serdes.Serialize(stream, obj);
        }

        public T? Deserialize(Stream stream, IDeserializationContext context)
        {
            if (stream.ReadByte() > 0)
                return _serdes.Deserialize(stream, context);
            return null;
        }
    }

    /// <summary>
    /// Extensions for serdes to add nullability support.
    /// </summary>
    public static class NullableSerdesExtensions
    {
        /// <summary>
        /// Creates a new serdes that adds nullability checks to this one.
        /// </summary>
        /// <typeparam name="T">The non-null type</typeparam>
        /// <param name="serdes">The serdes</param>
        /// <returns>The new nullable serdes</returns>
        public static ISerdes<T?> Nullable<T>(this ISerdes<T> serdes) where T : class
        {
            return new NullableSerdes<T>(serdes);
        }
    }
}