using System;
using System.Collections.Generic;
using System.Linq;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public interface IVertexConsumer<in T> where T : unmanaged
    {
        void Accept(T vertex);
        void Accept(IEnumerable<T> vertices);
    }

    public sealed class VertexTransformer<T> : IVertexConsumer<T> where T : unmanaged
    {
        private readonly IVertexConsumer<T> _next;
        private readonly Func<T, T> _transform;

        public VertexTransformer(IVertexConsumer<T> next, Func<T, T> transform)
        {
            _next = next;
            _transform = transform;
        }
        
        public void Accept(T vertex)
        {
            _next.Accept(_transform(vertex));
        }

        public void Accept(IEnumerable<T> vertices)
        {
            _next.Accept(vertices.Select(_transform));
        }
    }

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
}