using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// An empty serdes implementation that simply instantiates the type on deserialization.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public sealed class EmptySerdes<T> : ISerdes<T> where T : new()
    {
        public static ISerdes<T> Instance = new EmptySerdes<T>();

        private EmptySerdes()
        {
        }

        public void Serialize(Stream stream, T obj)
        {
        }

        public T Deserialize(Stream stream, IDeserializationContext context)
        {
            return new T();
        }
    }
}