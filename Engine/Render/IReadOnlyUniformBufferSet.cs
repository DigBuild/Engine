using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A read-only view of a uniform buffer set.
    /// </summary>
    public interface IReadOnlyUniformBufferSet
    {
        /// <summary>
        /// Gets the uniform buffer for a given handle.
        /// </summary>
        /// <typeparam name="TUniform">The uniform type</typeparam>
        /// <param name="uniformType">The uniform</param>
        /// <returns>The uniform buffer</returns>
        UniformBuffer<TUniform> Get<TUniform>(UniformType<TUniform> uniformType)
            where TUniform : unmanaged, IUniform<TUniform>;

        /// <summary>
        /// Gets the index within the uniform buffer for a given handle.
        /// </summary>
        /// <typeparam name="TUniform">The uniform type</typeparam>
        /// <param name="uniformType">The uniform</param>
        /// <returns>The uniform buffer</returns>
        uint GetIndex<TUniform>(UniformType<TUniform> uniformType)
            where TUniform : unmanaged, IUniform<TUniform>;
    }
}