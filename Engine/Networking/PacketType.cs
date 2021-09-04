using System.IO;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Networking
{
    /// <summary>
    /// A packet type.
    /// </summary>
    public interface IPacketType
    {
        /// <summary>
        /// The name.
        /// </summary>
        ResourceName Name { get; }

        /// <summary>
        /// Deserializes a packet from a bit stream.
        /// </summary>
        /// <param name="stream">The bit stream</param>
        /// <returns>A new packet</returns>
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

        /// <summary>
        /// Serializes a packet into a bit stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="packet">The packet</param>
        public void Serialize(Stream stream, T packet)
        {
            Serdes.Serialize(stream, packet);
        }

        public IPacket Deserialize(Stream stream)
        {
            return Serdes.Deserialize(stream, new SimpleDeserializationContext());
        }
    }
}