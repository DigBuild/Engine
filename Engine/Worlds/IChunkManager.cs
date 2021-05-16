using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public interface IChunkManager
    {
        bool TryLoad(IEnumerable<ChunkPos> chunks, bool immediate, [MaybeNullWhen(false)] out IChunkClaim claim);
    }
}