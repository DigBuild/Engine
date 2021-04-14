using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public interface IReadOnlyChunkBlockLight
    {
        byte Get(ChunkBlockPosition pos);
    }

    public interface IChunkBlockLight : IReadOnlyChunkBlockLight, IData<IChunkBlockLight>, IChangeNotifier
    {
        static DataHandle<IChunk, IReadOnlyChunkBlockLight, IChunkBlockLight> Type { get; set; } = null!;
    }

    public static class ChunkBlockLightExtensions
    {
        public static byte GetLight(this IReadOnlyWorld world, BlockPos pos)
        {
            var chunk = world.GetChunk(pos.ChunkPos);
            return chunk?.Get(IChunkBlockLight.Type).Get(pos.SubChunkPos) ?? 0;
        }
    }
}