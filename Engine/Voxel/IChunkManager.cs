using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public interface IChunkManager
    {
        public bool RequestLoadingTicket([MaybeNullWhen(false)] out IChunkLoadingTicket ticket, params ChunkPos[] chunkPositions)
        {
            return RequestLoadingTicket(out ticket, (IEnumerable<ChunkPos>) chunkPositions);
        }

        public bool RequestLoadingTicket([MaybeNullWhen(false)] out IChunkLoadingTicket ticket, IEnumerable<ChunkPos> chunkPositions);
    }
}