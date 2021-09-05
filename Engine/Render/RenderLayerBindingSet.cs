using System.Collections.Generic;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A set of render layer bindings.
    /// </summary>
    public sealed class RenderLayerBindingSet
    {
        private readonly Dictionary<IRenderLayer, object> _bindings = new();

        /// <summary>
        /// Sets the bindings for a layer.
        /// </summary>
        /// <typeparam name="TVertex">The vertex format</typeparam>
        /// <typeparam name="TBindings">The binding format</typeparam>
        /// <param name="layer">The layer</param>
        /// <param name="bindings">The bindings</param>
        public void Set<TVertex, TBindings>(IRenderLayer<TVertex, TBindings> layer, TBindings bindings)
            where TVertex : unmanaged
        {
            _bindings[layer] = bindings!;
        }

        /// <summary>
        /// Gets the bindings for a layer.
        /// </summary>
        /// <typeparam name="TVertex">The vertex type</typeparam>
        /// <typeparam name="TBindings">The binding type</typeparam>
        /// <param name="layer">The layer</param>
        /// <returns>The bindings</returns>
        public TBindings Get<TVertex, TBindings>(IRenderLayer<TVertex, TBindings> layer)
            where TVertex : unmanaged
        {
            return (TBindings)_bindings[layer];
        }
    }
}