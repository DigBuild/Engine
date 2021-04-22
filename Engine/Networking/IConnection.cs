using System.Threading.Tasks;

namespace DigBuild.Engine.Networking
{
    public interface IConnection
    {
        void Close();
        
        void Send<T>(T packet) where T : IPacket;
        Task SendAsync<T>(T packet) where T : IPacket;
    }
}