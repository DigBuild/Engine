using System.IO;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Networking
{
    public interface IPacketType
    {
        ResourceName Name { get; }

        IPacket Deserialize(Stream stream);
    }

    public class PacketType<T> : IPacketType where T : IPacket
    {
        public ResourceName Name { get; }
        private ISerdes<T> Serdes { get; }

        internal PacketType(ResourceName name, ISerdes<T> serdes)
        {
            Name = name;
            Serdes = serdes;
        }

        public void Serialize(Stream stream, T packet)
        {
            Serdes.Serialize(stream, packet);
        }

        public IPacket Deserialize(Stream stream)
        {
            return Serdes.Deserialize(stream);
        }
    }
}