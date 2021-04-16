using System.IO;

namespace DigBuild.Engine.Serialization
{
    public sealed class EmptySerdes<T> : ISerdes<T> where T : new()
    {
        public static ISerdes<T> Instance = new EmptySerdes<T>();

        public void Serialize(Stream stream, T obj)
        {
        }

        public T Deserialize(Stream stream)
        {
            return new();
        }
    }
}