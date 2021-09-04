using DigBuild.Engine.Registries;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Networking
{
    /// <summary>
    /// A network packet.
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// Handles the current packet for the given connection.
        /// </summary>
        /// <param name="connection">The connection</param>
        void Handle(IConnection connection);
    }

    /// <summary>
    /// Registry extensions for network packets.
    /// </summary>
    public static class PacketRegistryExtensions
    {
        /// <summary>
        /// Registers a new network packet.
        /// </summary>
        /// <typeparam name="T">The packet type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="domain">The domain</param>
        /// <param name="path">The path</param>
        /// <param name="serdes">The serdes</param>
        /// <returns></returns>
        public static PacketType<T> Register<T>(
            this ITypeRegistryBuilder<IPacket, IPacketType> builder,
            string domain, string path,
            ISerdes<T> serdes
        )
            where T : IPacket
        {
            return Register(builder, new ResourceName(domain, path), serdes);
        }
        
        /// <summary>
        /// Registers a new network packet.
        /// </summary>
        /// <typeparam name="T">The packet type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="name">The name</param>
        /// <param name="serdes">The serdes</param>
        /// <returns></returns>
        public static PacketType<T> Register<T>(
            this ITypeRegistryBuilder<IPacket, IPacketType> builder,
            ResourceName name,
            ISerdes<T> serdes
        )
            where T : IPacket
        {
            return builder.Add(typeof(T), new PacketType<T>(name, serdes));
        }
    }
}