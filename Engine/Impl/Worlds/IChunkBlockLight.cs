using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public interface IReadOnlyChunkBlockLight : IChangeNotifier
    {
        byte Get(ChunkBlockPos pos);
    }

    public interface IChunkBlockLight : IReadOnlyChunkBlockLight, IData<IChunkBlockLight>
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