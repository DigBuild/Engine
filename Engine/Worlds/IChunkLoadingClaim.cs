using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    /// <summary>
    /// A chunk loading claim.
    /// </summary>
    public interface IChunkLoadingClaim
    {
        /// <summary>
        /// The chunk positions.
        /// </summary>
        IEnumerable<ChunkPos> Chunks { get; }

        /// <summary>
        /// Releases the claim.
        /// </summary>
        void Release();
    }
}