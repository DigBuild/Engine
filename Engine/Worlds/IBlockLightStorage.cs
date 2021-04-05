using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyBlockLightStorage : IReadOnlyChunkStorage
    {
        byte Get(SubChunkPos pos);
    }

    public interface IBlockLightStorage : IReadOnlyBlockLightStorage, IChunkStorage<IBlockLightStorage>
    {
        static ChunkStorageType<IReadOnlyBlockLightStorage, IBlockLightStorage> Type { get; set; } = null!;
    }

    public static class BlockLightWorldExtensions
    {
        public static byte GetLight(this IReadOnlyWorld world, BlockPos pos)
        {
            var chunk = world.GetChunk(pos.ChunkPos);
            return chunk?.Get(IBlockLightStorage.Type).Get(pos.SubChunkPos) ?? 0;
        }
    }
}