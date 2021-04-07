using System.IO;
using System.Runtime.InteropServices;

namespace DigBuild.Engine.Serialization
{
    public static class UnmanagedSerdes<T> where T : unmanaged
    {
        public static ISerdes<T> NotNull { get; } = new NotNullImpl();
        public static ISerdes<T?> Nullable { get; } = new NullableImpl();

        private sealed class NotNullImpl : ISerdes<T>
        {
            public void Serialize(Stream stream, T obj)
            {
                var span = MemoryMarshal.CreateSpan(ref obj, 1);
                stream.Write(MemoryMarshal.AsBytes(span));
            }

            public T Deserialize(Stream stream)
            {
                var obj = default(T);
                var span = MemoryMarshal.CreateSpan(ref obj, 1);
                stream.Read(MemoryMarshal.AsBytes(span));
                return obj;
            }
        }

        private sealed class NullableImpl : ISerdes<T?>
        {
            public void Serialize(Stream stream, T? obj)
            {
                stream.WriteByte((byte) (obj.HasValue ? 1 : 0));
                if (obj.HasValue)
                    NotNull.Serialize(stream, obj.Value);
            }

            public T? Deserialize(Stream stream)
            {
                if (stream.ReadByte() > 0)
                    return NotNull.Deserialize(stream);
                return null;
            }
        }
    }
}