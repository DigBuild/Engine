using System.IO;

namespace DigBuild.Engine.Serialization
{
    public interface ISerdes<T> : IDeserializer<T>
    {
        void Serialize(Stream stream, T obj);
    }

    public static class SerdesExtensions
    {
        public static ISerdes<T2> UncheckedSuperCast<T, T2>(this ISerdes<T> serdes)
            where T : T2
            where T2 : class
        {
            return new SimpleSerdes<T2>(
                (stream, obj) => serdes.Serialize(stream, (T) obj),
                (stream, ctx) => serdes.Deserialize(stream, ctx)
            );
        }
    }
}