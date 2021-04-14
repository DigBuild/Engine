using System.Runtime.CompilerServices;

namespace DigBuild.Engine.Math
{
    public static class WorldDimensions
    {
        public const uint ChunkSize = 16; // 16^3 blocks
        public const uint RegionSize = 64; // 64^3 chunks
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BlockCoordToChunkCoord(int blockCoord) => blockCoord >> 4;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BlockCoordToSubChunkCoord(int blockCoord) => blockCoord & 0xF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChunkCoordToBlockCoord(int chunkCoord) => chunkCoord << 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChunkAndSubChunkCoordToBlockCoord(int chunkCoord, int subChunkCoord) => (chunkCoord << 4) | subChunkCoord;
    }
}