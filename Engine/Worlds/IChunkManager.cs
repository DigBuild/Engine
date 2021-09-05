using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    /// <summary>
    /// A chunk manager.
    /// </summary>
    public interface IChunkManager
    {
        /// <summary>
        /// Attempts to claim the given set of chunks, either immediately or progressively.
        /// </summary>
        /// <param name="chunks">The chunks</param>
        /// <param name="loadImmediately">Whether they should be immediately loaded</param>
        /// <param name="claim">The chunk loading claim</param>
        /// <returns>Whether the chunks were claimed or not</returns>
        bool TryClaim(IEnumerable<ChunkPos> chunks, bool loadImmediately, [MaybeNullWhen(false)] out IChunkLoadingClaim claim);
    }
}