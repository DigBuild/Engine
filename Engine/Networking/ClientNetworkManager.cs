using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Networking
{
    public sealed class ClientNetworkManager : IDisposable
    {
        public Connection Connection { get; }

        public ClientNetworkManager(
            string hostname, int port,
            IExtendedTypeRegistry<IPacket, IPacketType> packetTypes
        )
        {
            var client = new TcpClient(hostname, port);

            var packetTypesByName = packetTypes.Values.ToDictionary(type => type.Name);
            var packetTypesById = new Dictionary<ushort, IPacketType>();
            var packetIdsByType = new Dictionary<IPacketType, ushort>();

            var stream = client.GetStream();
            var br = new BinaryReader(stream);

            var packetIdCount = br.ReadUInt16();
            for (ushort i = 0; i < packetIdCount; i++)
            {
                var name = ResourceName.Parse(br.ReadString())!.Value;
                if (!packetTypesByName.TryGetValue(name, out var type))
                    continue;
                packetTypesById.Add(i, type);
                packetIdsByType.Add(type, i);
            }

            Connection = new Connection(client, packetTypes, packetTypesById, packetIdsByType);
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        public void Send<T>(T packet) where T : IPacket
        {
            Connection.Send(packet);
        }
    }
}