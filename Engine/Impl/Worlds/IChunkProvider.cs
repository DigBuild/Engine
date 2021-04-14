using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Impl.Worlds
{
    public interface IChunkProvider
    {
        bool TryGet(ChunkPos pos, [NotNullWhen(true)] out Chunk? chunk);
    }
}