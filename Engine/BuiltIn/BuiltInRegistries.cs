using DigBuild.Engine.Blocks;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInRegistries
    {
        public static Registry<IChunkStorageType> ChunkStorageTypes { get; set; } = null!;

        public static ExtendedTypeRegistry<IBlockEvent, BlockEventInfo> BlockEvents { get; set; } = null!;
        public static Registry<IBlockAttribute> BlockAttributes { get; set; } = null!;
        public static Registry<IBlockCapability> BlockCapabilities { get; set; } = null!;
    }
}