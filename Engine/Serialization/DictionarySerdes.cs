using System.Collections.Generic;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    public class DictionarySerdes<TK, TV> : ISerdes<Dictionary<TK, TV>> where TK : notnull
    {
        private readonly ISerdes<TK> _keySerdes;
        private readonly ISerdes<TV> _valueSerdes;

        public DictionarySerdes(ISerdes<TK> keySerdes, ISerdes<TV> valueSerdes)
        {
            _keySerdes = keySerdes;
            _valueSerdes = valueSerdes;
        }

        public void Serialize(Stream stream, Dictionary<TK, TV> dictionary)
        {
            var bw = new BinaryWriter(stream);
            bw.Write(dictionary.Count);
            foreach (var (key, value) in dictionary)
            {
                _keySerdes.Serialize(stream, key);
                _valueSerdes.Serialize(stream, value);
            }
        }

        public Dictionary<TK, TV> Deserialize(Stream stream, IDeserializationContext context)
        {
            var br = new BinaryReader(stream);
            var count = br.ReadInt32();
            var dictionary = new Dictionary<TK, TV>(count);
            for (var i = 0; i < count; i++)
            {
                var key = _keySerdes.Deserialize(stream, context);
                var value = _valueSerdes.Deserialize(stream, context);
                dictionary[key] = value;
            }
            return dictionary;
        }
    }
}