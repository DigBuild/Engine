using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    /// <summary>
    /// A uniform buffer set writer.
    /// </summary>
    public interface IUniformBufferSetWriter
    {
        /// <summary>
        /// Pushes a new uniform to the corresponding buffer and returns its index.
        /// </summary>
        /// <typeparam name="TUniform">The uniform type</typeparam>
        /// <param name="uniformType">The handle</param>
        /// <param name="value">The value</param>
        /// <returns>The index</returns>
        uint Push<TUniform>(UniformType<TUniform> uniformType, TUniform value)
            where TUniform : unmanaged, IUniform<TUniform>;
    }
}