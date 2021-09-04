using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.Networking
{
    /// <summary>
    /// A network connection.
    /// </summary>
    public sealed class Connection : IConnection, IDisposable
    {
        private readonly TcpClient _client;

        private readonly TypeRegistry<IPacket, IPacketType> _packetTypes;
        private readonly Dictionary<ushort, IPacketType> _packetTypesById;
        private readonly Dictionary<IPacketType, ushort> _packetIdsByType;

        private readonly Thread _rxThread, _txThread;
        private readonly BlockingCollection<Packet> _packetQueue = new();

        public bool Connected => _client.Connected;

        /// <summary>
        /// Fired when the connection is closed.
        /// </summary>
        public event Action? Closed;

        public Connection(
            TcpClient client,
            string name,
            TypeRegistry<IPacket, IPacketType> packetTypes,
            Dictionary<ushort, IPacketType> packetTypesById,
            Dictionary<IPacketType, ushort> packetIdsByType
        )
        {
            _client = client;
            _packetTypes = packetTypes;
            _packetTypesById = packetTypesById;
            _packetIdsByType = packetIdsByType;
            
            _rxThread = new Thread(() =>
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
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Exception while handling packet: {ex.Message}");
                        break;
                    }
                }

                Closed?.Invoke();
            }) { Name = $"Network RX: {name}" };

            _txThread = new Thread(() =>
            {
                var stream = _client.GetStream();
                var bw = new BinaryWriter(stream);

                while (_client.Connected)
                {
                    var packet = _packetQueue.Take();
                    if (packet.Id == ushort.MaxValue)
                        continue;

                    try
                    {
                        bw.Write(packet.Id);
                        bw.Write(packet.Bytes, 0, packet.Length);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Exception while handling packet: {ex.Message}");
                        break;
                    }
                }

                Closed?.Invoke();
            }) { Name = $"Network TX: {name}" };
        }

        /// <summary>
        /// Starts the packet handling threads.
        /// </summary>
        public void StartHandlingPackets()
        {
            _rxThread.Start();
            _txThread.Start();
        }

        public void Close()
        {
            _client.Close();
        }

        public void Dispose()
        {
            Close();
            Enqueue(ushort.MaxValue, null!, 0); // Unblock tx queue
            _txThread.Join();
            _rxThread.Join();
            _client.Dispose();
        }

        internal void Enqueue(IPacketType type, byte[] bytes, int length)
        {
            if (!_packetIdsByType.TryGetValue(type, out var id))
                throw new ArgumentException($"Unsupported packet type: {type.Name}", nameof(type));

            Enqueue(id, bytes, length);
        }

        public void Send<T>(T packet) where T : IPacket
        {
            if (!_packetTypes.TryGetValue(typeof(T), out var t) || t is not PacketType<T> type)
                throw new ArgumentException($"Cannot send unregistered packet type: {packet.GetType().FullName}", nameof(packet));
            if (!_packetIdsByType.TryGetValue(type, out var id))
                throw new ArgumentException($"Unsupported packet type: {type.Name}", nameof(packet));
            
            var serialized = Serialize(type, packet);
            Enqueue(id, serialized.Bytes, serialized.Length);
        }

        public Task SendAsync<T>(T packet) where T : IPacket
        {
            return Task.Run(() => Send(packet));
        }

        private void Enqueue(ushort id, byte[] bytes, int length)
        {
            _packetQueue.Add(new Packet(id, bytes, length));
        }

        internal static (byte[] Bytes, int Length) Serialize<T>(PacketType<T> type, T packet) where T : IPacket
        {
            var stream = new MemoryStream();
            type.Serialize(stream, packet);
            return (stream.GetBuffer(), (int) stream.Position);
        }

        private sealed class Packet
        {
            public ushort Id { get; }
            public byte[] Bytes { get; }
            public int Length { get; }

            public Packet(ushort id, byte[] bytes, int length)
            {
                Id = id;
                Bytes = bytes;
                Length = length;
            }
        }
    }
}