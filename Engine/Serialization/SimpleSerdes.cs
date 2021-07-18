using System;
using System.Collections.Generic;
using System.IO;

namespace DigBuild.Engine.Serialization
{
    public sealed class SimpleSerdes<T> : ISerdes<T>
    {
        private readonly Action<Stream, T> _serialize;
        private readonly Func<Stream, T> _deserialize;

        public SimpleSerdes(Action<Stream, T> serialize, Func<Stream, T> deserialize)
        {
            _serialize = serialize;
            _deserialize = deserialize;
        }

        public void Serialize(Stream stream, T obj)
        {
            _serialize(stream, obj);
        }

        public T Deserialize(Stream stream)
        {
            return _deserialize(stream);
        }
    }

    public static class SimpleSerdes
    {
        public static ISerdes<List<T>> OfList<T>(ISerdes<T> serdes)
        {
            return new SimpleSerdes<List<T>>(
                (stream, list) =>
                {
                    var bw = new BinaryWriter(stream);
                    bw.Write(list.Count);
                    foreach (var entry in list)
                        serdes.Serialize(stream, entry);
                },
                stream =>
                {
                    var br = new BinaryReader(stream);
                    var count = br.ReadInt32();
                    var list = new List<T>();
                    for (var i = 0; i < count; i++)
                        list.Add(serdes.Deserialize(stream));
                    return list;
                }
            );
        }
    }
}