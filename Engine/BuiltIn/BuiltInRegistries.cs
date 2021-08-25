using DigBuild.Engine.Blocks;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Items;
using DigBuild.Engine.Registries;

namespace DigBuild.Engine.BuiltIn
{
    internal static class BuiltInRegistries
    {
        public static Registry<Block> Blocks { get; set; } = null!;
        public static TypeRegistry<IBlockEvent, BlockEventInfo> BlockEvents { get; set; } = null!;
        public static Registry<IBlockAttribute> BlockAttributes { get; set; } = null!;
        public static Registry<IBlockCapability> BlockCapabilities { get; set; } = null!;

        public static Registry<Item> Items { get; set; } = null!;
        public static TypeRegistry<IItemEvent, ItemEventInfo> ItemEvents { get; set; } = null!;
        public static Registry<IItemAttribute> ItemAttributes { get; set; } = null!;
        public static Registry<IItemCapability> ItemCapabilities { get; set; } = null!;

        public static Registry<Entity> Entities { get; set; } = null!;
        public static TypeRegistry<IEntityEvent, EntityEventInfo> EntityEvents { get; set; } = null!;
        public static Registry<IEntityAttribute> EntityAttributes { get; set; } = null!;
        public static Registry<IEntityCapability> EntityCapabilities { get; set; } = null!;

        public static void Initialize()
        {
            DigBuildEngine.SubscribeBuilt<Block>(reg => Blocks = reg);
            DigBuildEngine.SubscribeBuilt<IBlockEvent, BlockEventInfo>(reg => BlockEvents = reg);
            DigBuildEngine.SubscribeBuilt<IBlockAttribute>(reg => BlockAttributes = reg);
            DigBuildEngine.SubscribeBuilt<IBlockCapability>(reg => BlockCapabilities = reg);

            DigBuildEngine.SubscribeBuilt<Item>(reg => Items = reg);
            DigBuildEngine.SubscribeBuilt<IItemEvent, ItemEventInfo>(reg => ItemEvents = reg);
            DigBuildEngine.SubscribeBuilt<IItemAttribute>(reg => ItemAttributes = reg);
            DigBuildEngine.SubscribeBuilt<IItemCapability>(reg => ItemCapabilities = reg);

            DigBuildEngine.SubscribeBuilt<Entity>(reg => Entities = reg);
            DigBuildEngine.SubscribeBuilt<IEntityEvent, EntityEventInfo>(reg => EntityEvents = reg);
            DigBuildEngine.SubscribeBuilt<IEntityAttribute>(reg => EntityAttributes = reg);
            DigBuildEngine.SubscribeBuilt<IEntityCapability>(reg => EntityCapabilities = reg);

        }
    }
}