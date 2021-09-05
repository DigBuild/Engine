using System;
using System.Collections.Generic;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A vertex consumer that adds vertices to a native buffer.
    /// </summary>
    /// <typeparam name="T">The vertex type.</typeparam>
    public sealed class NativeBufferVertexConsumer<T> : IVertexConsumer<T> where T : unmanaged
    {
        private readonly INativeBuffer<T> _buffer;

        public NativeBufferVertexConsumer(INativeBuffer<T> buffer)
        {
            _buffer = buffer;
        }

        public void Accept(T vertex) => _buffer.Add(vertex);
        public void Accept(IEnumerable<T> vertices) => _buffer.Add(vertices);
    }

    /// <summary>
    /// A vertex consumer that adds vertices to a lazily initialized native buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LazyNativeBufferVertexConsumer<T> : IVertexConsumer<T> where T : unmanaged
    {
        private readonly Func<INativeBuffer<T>> _supplier;
        private INativeBuffer<T>? _buffer;

        public LazyNativeBufferVertexConsumer(Func<INativeBuffer<T>> supplier)
        {
            _supplier = supplier;
        }

        public void Accept(T vertex)
        {
            _buffer ??= _supplier();
            _buffer.Add(vertex);
        }

        public void Accept(IEnumerable<T> vertices)
        {
            _buffer ??= _supplier();
            _buffer.Add(vertices);
        }
    }
}