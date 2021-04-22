using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Networking
{
    public interface IPacket
    {
        void Handle(IConnection connection);
    }

    public static class PacketExtensions
    {
        public static PacketType<T> Register<T>(
            this IExtendedTypeRegistryBuilder<IPacket, IPacketType> builder,
            string domain, string path,
            ISerdes<T> serdes
        )
            where T : IPacket
        {
            return Register(builder, new ResourceName(domain, path), serdes);
        }

        public static PacketType<T> Register<T>(
            this IExtendedTypeRegistryBuilder<IPacket, IPacketType> builder,
            ResourceName name,
            ISerdes<T> serdes
        )
            where T : IPacket
        {
            return builder.Add(typeof(T), new PacketType<T>(name, serdes));
        }
    }
}