using System.IO;

namespace DigBuild.Engine.Serialization
{
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

        public T? Deserialize(Stream stream)
        {
            if (stream.ReadByte() > 0)
                return _serdes.Deserialize(stream);
            return null;
        }
    }

    public static class NullableSerdesExtensions
    {
        public static ISerdes<T?> Nullable<T>(this ISerdes<T> serdes) where T : class
        {
            return new NullableSerdes<T>(serdes);
        }
    }
}