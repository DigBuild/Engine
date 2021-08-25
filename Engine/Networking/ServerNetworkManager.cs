using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Networking
{
    public sealed class ServerNetworkManager : IDisposable
    {
        private readonly TypeRegistry<IPacket, IPacketType> _packetTypes;

        private readonly TcpListener _listener;
        private readonly HashSet<Connection> _connections = new();

        private readonly Thread _thread;
        private bool _threadActive = true;

        public event Action<Connection>? ClientConnected;

        public ServerNetworkManager(
            int port,
            TypeRegistry<IPacket, IPacketType> packetTypes
        )
        {
            _packetTypes = packetTypes;

            _listener = new TcpListener(new IPAddress(0), port);

            var packetList = packetTypes.Values.ToList();
            var indexedPacketList = packetList.Select((name, index) => new {Index = (ushort) index, Name = name}).ToList();
            var packetTypesById = indexedPacketList.ToDictionary(x => x.Index, x => x.Name);
            var packetIdsByType = indexedPacketList.ToDictionary(x => x.Name, x => x.Index);
            
            _thread = new Thread(() =>
            {
                _listener.Start();
                while (true)
                {
                    while (_threadActive && !_listener.Pending())
                        Thread.Sleep(100);
                    if (!_threadActive)
                        break;

                    var client = _listener.AcceptTcpClient();
                    var stream = client.GetStream();
                    var bw = new BinaryWriter(stream);

                    bw.Write((ushort) packetList.Count);
                    foreach (var type in packetList)
                        bw.Write(type.Name.ToString());

                    var connection = new Connection(client, $"Client hash {client.GetHashCode()}", packetTypes, packetTypesById, packetIdsByType);
                    lock (_connections)
                    {
                        _connections.Add(connection);
                    }
                    connection.Closed += () =>
                    {
                        lock (_connections)
                        {
                            _connections.Remove(connection);
                        }
                    };
                    connection.StartHandlingPackets();

                    ClientConnected?.Invoke(connection);
                }
            });
            _thread.Start();
        }

        public void Close()
        {
            _threadActive = false;
            _listener.Stop();
            foreach (var connection in _connections)
                connection.Close();
        }

        public void Dispose()
        {
            Close();
            foreach (var connection in _connections)
                connection.Dispose();
            _thread.Join();
        }

        public void SendToAll<T>(T packet) where T : IPacket
        {
            if (!_packetTypes.TryGetValue(typeof(T), out var t) || t is not PacketType<T> type)
                throw new ArgumentException($"Cannot send unregistered packet type: {packet.GetType().FullName}", nameof(packet));

            var serialized = Connection.Serialize(type, packet);

            lock (_connections)
            {
                foreach (var connection in _connections)
                    connection.Enqueue(type, serialized.Bytes, serialized.Length);
            }
        }

        public Task SendToAllAsync<T>(T packet) where T : IPacket
        {
            return Task.Run(() => SendToAll(packet));
        }
    }
}