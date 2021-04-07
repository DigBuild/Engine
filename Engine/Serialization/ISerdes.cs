using System.IO;

namespace DigBuild.Engine.Serialization
{
    public interface ISerdes<T> : IDeserializer<T>
    {
        void Serialize(Stream stream, T obj);
    }
}