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

        public static EventBus EventBus { get; set; } = null!;

        public static void Register(RegistryBuilder<IDataHandle<IWorld>> registry)
        {
            WorldEntities.Type = registry.Create<IWorld, IReadOnlyWorldEntities, WorldEntities>(
                new ResourceName(Domain, "entities"), WorldEntities.Serdes
            );
        }

        public static void Register(RegistryBuilder<IDataHandle<IChunk>> registry)
        {
            ChunkBlocks.Type = registry.Create<IChunk, IReadOnlyChunkBlocks, ChunkBlocks>(
                new ResourceName(Domain, "blocks"), ChunkBlocks.Serdes
            );
            ChunkEntities.Type = registry.Create<IChunk, IReadOnlyChunkEntities, ChunkEntities>(
                new ResourceName(Domain, "entities"), ChunkEntities.Serdes
            );
        }

        public static void Register(RegistryBuilder<IBlockAttribute> registry)
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

        public static void Register(RegistryBuilder<IItemAttribute> registry)
        {
            ModelData.ItemAttribute = registry.Register(
                new ResourceName(Domain, "model_data"),
                () => new ModelData()
            );
        }

        public static void Register(RegistryBuilder<IEntityAttribute> registry)
        {
            ModelData.EntityAttribute = registry.Register(
                new ResourceName(Domain, "model_data"),
                () => new ModelData()
            );
        }

        public static void Register(ExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry)
        {
            registry.Register((BuiltInBlockEvent.JoinedWorld _) => { });
            registry.Register((BuiltInBlockEvent.LeavingWorld _) => { });
        }

        public static void Register(ExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry)
        {
            registry.Register((BuiltInEntityEvent.JoinedWorld _) => { });
            registry.Register((BuiltInEntityEvent.LeavingWorld _) => { });
        }
    }
}