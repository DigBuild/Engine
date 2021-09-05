using System.Runtime.CompilerServices;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// World dimension specification and helpers.
    /// </summary>
    public static class WorldDimensions
    {
        public const uint ChunkWidth = 16; // 16^2 blocks
        public const uint ChunkVerticalSubdivisions = 16;
        public const uint ChunkHeight = ChunkWidth * ChunkVerticalSubdivisions; // 16 * 16 sub-chunks
        public const uint RegionSize = 64; // 64^3 chunks
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int XZBlockCoordToChunkCoord(int blockCoord) => blockCoord >> 4;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int XZBlockCoordToSubChunkCoord(int blockCoord) => blockCoord & 0xF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int XZChunkCoordToBlockCoord(int chunkCoord) => chunkCoord << 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int XZChunkAndSubChunkCoordToBlockCoord(int chunkCoord, int subChunkCoord) => (chunkCoord << 4) | subChunkCoord;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChunkCoordToRegionCoord(int chunkCoord) => chunkCoord >> 6;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChunkCoordToSubRegionCoord(int chunkCoord) => chunkCoord & 0x3F;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RegionCoordToChunkCoord(int regionCoord) => regionCoord << 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RegionAndSubRegionCoordToChunkCoord(int regionCoord, int subRegionCoord) => (regionCoord << 6) | subRegionCoord;
    }
}