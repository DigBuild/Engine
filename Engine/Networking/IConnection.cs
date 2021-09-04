using System.Threading.Tasks;

namespace DigBuild.Engine.Networking
{
    /// <summary>
    /// A network connection.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Whether the connection is active or not.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();
        
        /// <summary>
        /// Sends a packet to the other side.
        /// </summary>
        /// <typeparam name="T">The packet type</typeparam>
        /// <param name="packet">The packet</param>
        void Send<T>(T packet) where T : IPacket;
        /// <summary>
        /// Asynchronously sends a packet to the other side.
        /// </summary>
        /// <typeparam name="T">The packet type</typeparam>
        /// <param name="packet">The packet</param>
        /// <returns>A task that will be completed when the packet is sent</returns>
        Task SendAsync<T>(T packet) where T : IPacket;
    }
}