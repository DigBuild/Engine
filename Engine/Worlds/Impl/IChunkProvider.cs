using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds.Impl
{
    public interface IChunkProvider
    {
        bool TryGet(ChunkPos pos, [NotNullWhen(true)] out Chunk? chunk);
    }
}