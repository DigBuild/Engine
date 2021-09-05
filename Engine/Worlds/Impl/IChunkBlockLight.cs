using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A read-only view of the block lighting in a chunk.
    /// </summary>
    public interface IReadOnlyChunkBlockLight : IChangeNotifier
    {
        /// <summary>
        /// Gets the light value at a position in the chunk.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <returns>The light value</returns>
        byte Get(ChunkBlockPos pos);
    }
    
    /// <summary>
    /// A chunk data type for block lighting storage.
    /// </summary>
    public interface IChunkBlockLight : IReadOnlyChunkBlockLight, IData<IChunkBlockLight>
    {
        static DataHandle<IChunk, IReadOnlyChunkBlockLight, IChunkBlockLight> Type { get; set; } = null!;
    }

    /// <summary>
    /// Helpers for chunk block light.
    /// </summary>
    public static class ChunkBlockLightExtensions
    {
        /// <summary>
        /// Gets the block light value at a specific position in the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="pos">The position</param>
        /// <returns>The light value</returns>
        public static byte GetLight(this IReadOnlyWorld world, BlockPos pos)
        {
            var chunk = world.GetChunk(pos.ChunkPos);
            return chunk?.Get(IChunkBlockLight.Type).Get(pos.SubChunkPos) ?? 0;
        }
    }
}