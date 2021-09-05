using System.Collections.Generic;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A serdes implementation for lists.
    /// </summary>
    /// <typeparam name="T">The list element type</typeparam>
    public class ListSerdes<T> : ISerdes<List<T>>
    {
        private readonly ISerdes<T> _serdes;

        public ListSerdes(ISerdes<T> serdes)
        {
            _serdes = serdes;
        }

        public void Serialize(Stream stream, List<T> list)
        {
            var bw = new BinaryWriter(stream);
            bw.Write(list.Count);
            foreach (var element in list)
                _serdes.Serialize(stream, element);
        }

        public List<T> Deserialize(Stream stream, IDeserializationContext context)
        {
            var br = new BinaryReader(stream);
            var count = br.ReadInt32();
            var list = new List<T>(count);
            for (var i = 0; i < count; i++)
                list.Add(_serdes.Deserialize(stream, context));
            return list;
        }
    }
}