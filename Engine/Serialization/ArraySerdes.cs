using System.IO;

namespace DigBuild.Engine.Serialization
{
    /// <summary>
    /// A serdes implementation for arrays.
    /// </summary>
    /// <typeparam name="T">The array element type</typeparam>
    public class ArraySerdes<T> : ISerdes<T[]>
    {
        private readonly ISerdes<T> _serdes;

        public ArraySerdes(ISerdes<T> serdes)
        {
            _serdes = serdes;
        }

        public void Serialize(Stream stream, T[] array)
        {
            var bw = new BinaryWriter(stream);
            bw.Write(array.Length);
            foreach (var element in array)
                _serdes.Serialize(stream, element);
        }

        public T[] Deserialize(Stream stream, IDeserializationContext context)
        {
            var br = new BinaryReader(stream);
            var count = br.ReadInt32();
            var array = new T[count];
            for (var i = 0; i < count; i++)
                array[i] = _serdes.Deserialize(stream, context);
            return array;
        }
    }
}