using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A chunk provider.
    /// </summary>
    public interface IChunkProvider
    {
        /// <summary>
        /// Attempts to get the chunk at a given position.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="chunk">The chunk</param>
        /// <returns>Whether the chunk was found or not</returns>
        bool TryGet(ChunkPos pos, [NotNullWhen(true)] out Chunk? chunk);
    }
}