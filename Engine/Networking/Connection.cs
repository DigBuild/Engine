using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Networking
{
    public sealed class Connection : IDisposable
    {
        private readonly TcpClient _client;

        private readonly IExtendedTypeRegistry<IPacket, IPacketType> _packetTypes;
        private readonly Dictionary<ushort, IPacketType> _packetTypesById;
        private readonly Dictionary<IPacketType, ushort> _packetIdsByType;

        private readonly Thread _thread;

        public bool Connected => _client.Connected;

        public event Action? Closed;

        public Connection(
            TcpClient client,
            IExtendedTypeRegistry<IPacket, IPacketType> packetTypes,
            Dictionary<ushort, IPacketType> packetTypesById,
            Dictionary<IPacketType, ushort> packetIdsByType
        )
        {
            _client = client;
            _packetTypes = packetTypes;
            _packetTypesById = packetTypesById;
            _packetIdsByType = packetIdsByType;

            _thread = new Thread(() =>
            {
                var stream = _client.GetStream();
                var br = new BinaryReader(stream);

                while (_client.Connected)
                {
                    try
                    {
                        var id = br.ReadUInt16();
                        var type = _packetTypesById[id];
                        var packet = type.Deserialize(stream);
                        packet.Handle(this);
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }

                Closed?.Invoke();
            });
            _thread.Start();
        }

        public void Close()
        {
            _client.Close();
        }

        public void Dispose()
        {
            Close();
            _thread.Join();
            _client.Dispose();
        }

        public void Send<T>(T packet) where T : IPacket
        {
            if (!_packetTypes.TryGetValue(typeof(T), out var t) || t is not PacketType<T> type)
                throw new ArgumentException($"Cannot send unregistered packet type: {packet.GetType().FullName}", nameof(packet));
            if (!_packetIdsByType.TryGetValue(type, out var id))
                throw new ArgumentException($"Unsupported packet type on server: {type.Name}", nameof(packet));

            lock (_client)
            {
                var stream = _client.GetStream();
                var bw = new BinaryWriter(stream);

                bw.Write(id);
                type.Serialize(stream, packet);
            }
        }
    }
}