using DigBuild.Engine.Blocks;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Items;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInRegistries
    {
        public static Registry<IDataHandle<IWorld>> WorldStorageTypes { get; set; } = null!;
        public static Registry<IDataHandle<IChunk>> ChunkStorageTypes { get; set; } = null!;

        public static Registry<Block> Blocks { get; set; } = null!;
        public static ExtendedTypeRegistry<IBlockEvent, BlockEventInfo> BlockEvents { get; set; } = null!;
        public static Registry<IBlockAttribute> BlockAttributes { get; set; } = null!;
        public static Registry<IBlockCapability> BlockCapabilities { get; set; } = null!;

        public static Registry<Item> Items { get; set; } = null!;

        public static Registry<Entity> Entities { get; set; } = null!;
    }
}