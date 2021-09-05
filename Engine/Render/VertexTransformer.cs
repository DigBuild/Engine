using System;
using System.Collections.Generic;
using System.Linq;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A vertex consumer that applies a transform to all incoming vertices
    /// </summary>
    /// <typeparam name="T">The vertex type</typeparam>
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
}