using System.IO;

namespace DigBuild.Engine.Serialization
{
    public interface IDeserializer<out T>
    {
        T Deserialize(Stream stream);
    }
}