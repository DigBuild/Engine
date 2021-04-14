using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public interface IChunkClaim
    {
        IEnumerable<ChunkPos> Chunks { get; }

        void Release();
    }
}