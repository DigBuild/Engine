using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    internal sealed class DataContainerSerdes : ISerdes<DataContainer?>
    {
        private readonly IReadOnlyDictionary<IDataHandle, (ResourceName Name, ISerdes<IData> Serdes)> _serializedHandles;
        private readonly Dictionary<ResourceName, (IDataHandle Handle, ISerdes<IData> Serdes)> _serializedNames = new();

        public DataContainerSerdes(IReadOnlyDictionary<IDataHandle, (ResourceName Name, ISerdes<IData> Serdes)> serializedHandles)
        {
            _serializedHandles = serializedHandles;
            foreach (var (handle, (name, serdes)) in _serializedHandles)
                _serializedNames.Add(name, (handle, serdes));
        }

        public void Serialize(Stream stream, DataContainer? obj)
        {
            var bw = new BinaryWriter(stream);

            if (obj == null)
            {
                bw.Write(-1);
                return;
            }

            var count = obj.Entries.Keys.Count(_serializedHandles.ContainsKey);
            bw.Write(count);

            foreach (var (handle, data) in obj.Entries)
            {
                if (!_serializedHandles.TryGetValue(handle, out var entry))
                    continue;

                bw.Write(entry.Name.ToString());
                entry.Serdes.Serialize(stream, data);
            }
        }

        public DataContainer? Deserialize(Stream stream)
        {
            var br = new BinaryReader(stream);

            var count = br.ReadInt32();
            if (count == -1)
                return null;

            var entries = new Dictionary<IDataHandle, IData>();

            for (var i = 0; i < count; i++)
            {
                var name = ResourceName.Parse(br.ReadString())!.Value;

                if (!_serializedNames.TryGetValue(name, out var entry))
                    throw new Exception($"Could not find data handle with name: {name}");

                var data = entry.Serdes.Deserialize(stream);
                entries.Add(entry.Handle, data);
            }

            return new DataContainer(entries, false);
        }
    }
}