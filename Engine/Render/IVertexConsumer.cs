using System;
using System.Collections.Generic;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A vertex consumer.
    /// </summary>
    /// <typeparam name="T">The vertex type</typeparam>
    public interface IVertexConsumer<in T> where T : unmanaged
    {
        /// <summary>
        /// Accepts a single vertex.
        /// </summary>
        /// <param name="vertex">The vertex</param>
        void Accept(T vertex);

        /// <summary>
        /// Accepts an enumeration of vertices.
        /// </summary>
        /// <param name="vertices">The vertices</param>
        void Accept(IEnumerable<T> vertices);

        /// <summary>
        /// Accepts an array of vertices.
        /// </summary>
        /// <param name="vertices">The vertices</param>
        void Accept(params T[] vertices) => Accept((IEnumerable<T>) vertices);
    }
}