using System;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Events;
using DigBuild.Engine.Items;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Render.Models;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;
using DigBuild.Engine.Worlds.Impl;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine
{
    public static class DigBuildEngine
    {
        public const string Domain = "digbuildengine";

        internal static EventBus EventBus { get; private set; } = null!;
        
        internal static Registry<Block> Blocks { get; set; } = null!;
        internal static TypeRegistry<IBlockEvent, BlockEventInfo> BlockEvents { get; set; } = null!;
        internal static Registry<IBlockAttribute> BlockAttributes { get; set; } = null!;
        internal static Registry<IBlockCapability> BlockCapabilities { get; set; } = null!;

        internal static Registry<Item> Items { get; set; } = null!;
        internal static TypeRegistry<IItemEvent, ItemEventInfo> ItemEvents { get; set; } = null!;
        internal static Registry<IItemAttribute> ItemAttributes { get; set; } = null!;
        internal static Registry<IItemCapability> ItemCapabilities { get; set; } = null!;

        internal static Registry<Entity> Entities { get; set; } = null!;
        internal static TypeRegistry<IEntityEvent, EntityEventInfo> EntityEvents { get; set; } = null!;
        internal static Registry<IEntityAttribute> EntityAttributes { get; set; } = null!;
        internal static Registry<IEntityCapability> EntityCapabilities { get; set; } = null!;

        public static void Initialize(EventBus eventBus)
        {
            EventBus = eventBus;
            
            SubscribeRegister<IDataHandle<IWorld>>(Register);
            SubscribeRegister<IDataHandle<IChunk>>(Register);
            SubscribeRegister<IBlockAttribute>(Register);
            SubscribeRegister<IItemAttribute>(Register);
            SubscribeRegister<IEntityAttribute>(Register);
            SubscribeRegister<IBlockEvent, BlockEventInfo>(Register);
            SubscribeRegister<IEntityEvent, EntityEventInfo>(Register);
            
            SubscribeBuilt<Block>(reg => Blocks = reg);
            SubscribeBuilt<IBlockEvent, BlockEventInfo>(reg => BlockEvents = reg);
            SubscribeBuilt<IBlockAttribute>(reg => BlockAttributes = reg);
            SubscribeBuilt<IBlockCapability>(reg => BlockCapabilities = reg);

            SubscribeBuilt<Item>(reg => Items = reg);
            SubscribeBuilt<IItemEvent, ItemEventInfo>(reg => ItemEvents = reg);
            SubscribeBuilt<IItemAttribute>(reg => ItemAttributes = reg);
            SubscribeBuilt<IItemCapability>(reg => ItemCapabilities = reg);

            SubscribeBuilt<Entity>(reg => Entities = reg);
            SubscribeBuilt<IEntityEvent, EntityEventInfo>(reg => EntityEvents = reg);
            SubscribeBuilt<IEntityAttribute>(reg => EntityAttributes = reg);
            SubscribeBuilt<IEntityCapability>(reg => EntityCapabilities = reg);
            
            SubscribeBuilt<IDataHandle<IWorld>>(reg => DataContainer<IWorld>.Registry = reg);
            SubscribeBuilt<IDataHandle<IChunk>>(reg => DataContainer<IChunk>.Registry = reg);
        }

        private static void SubscribeRegister<T>(Action<RegistryBuilder<T>> reg) where T : notnull
            => EventBus.Subscribe<RegistryBuildingEvent<T>>(evt => reg(evt.Registry));
        private static void SubscribeRegister<T, TValue>(Action<TypeRegistryBuilder<T, TValue>> reg) where T : notnull
            => EventBus.Subscribe<TypeRegistryBuildingEvent<T, TValue>>(evt => reg(evt.Registry));

        internal static void SubscribeBuilt<T>(Action<Registry<T>> reg) where T : notnull
            => EventBus.Subscribe<RegistryBuiltEvent<T>>(evt => reg(evt.Registry));
        internal static void SubscribeBuilt<T, TValue>(Action<TypeRegistry<T, TValue>> reg) where T : notnull
            => EventBus.Subscribe<TypeRegistryBuiltEvent<T, TValue>>(evt => reg(evt.Registry));

        private static void Register(RegistryBuilder<IDataHandle<IWorld>> registry)
        {
            WorldEntities.Type = registry.Create<IWorld, IReadOnlyWorldEntities, WorldEntities>(
                new ResourceName(Domain, "entities"), WorldEntities.Serdes
            );
        }

        private static void Register(RegistryBuilder<IDataHandle<IChunk>> registry)
        {
            ChunkBlocks.Type = registry.Create<IChunk, IReadOnlyChunkBlocks, ChunkBlocks>(
                new ResourceName(Domain, "blocks"), ChunkBlocks.Serdes
            );
            ChunkEntities.Type = registry.Create<IChunk, IReadOnlyChunkEntities, ChunkEntities>(
                new ResourceName(Domain, "entities"), ChunkEntities.Serdes
            );
        }

        private static void Register(RegistryBuilder<IBlockAttribute> registry)
        {
            ModelData.BlockAttribute = registry.Register(
                new ResourceName(Domain, "model_data"),
                () => new ModelData()
            );
            BlockFaceSolidity.Attribute = registry.Register(
                new ResourceName(Domain, "face_solidity"),
                () => BlockFaceSolidity.All
            );
        }

        private static void Register(RegistryBuilder<IItemAttribute> registry)
        {
            ModelData.ItemAttribute = registry.Register(
                new ResourceName(Domain, "model_data"),
                () => new ModelData()
            );
        }

        private static void Register(RegistryBuilder<IEntityAttribute> registry)
        {
            ModelData.EntityAttribute = registry.Register(
                new ResourceName(Domain, "model_data"),
                () => new ModelData()
            );
        }

        private static void Register(TypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry)
        {
            registry.Register((BuiltInBlockEvent.JoinedWorld _) => { });
            registry.Register((BuiltInBlockEvent.LeavingWorld _) => { });
        }

        private static void Register(TypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry)
        {
            registry.Register((BuiltInEntityEvent.JoinedWorld _) => { });
            registry.Register((BuiltInEntityEvent.LeavingWorld _) => { });
        }
    }
}