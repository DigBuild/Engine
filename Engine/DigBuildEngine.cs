﻿using DigBuild.Engine.Blocks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Render;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine
{
    public static class DigBuildEngine
    {
        public const string Domain = "digbuildengine";

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
        }

        public static void Register(ExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry)
        {
            registry.Register((BuiltInBlockEvent.JoinedWorld _) => { });
            registry.Register((BuiltInBlockEvent.LeavingWorld _) => { });
        }

        public static void Register(RegistryBuilder<IBlockAttribute> registry)
        {
            ModelData.Attribute = registry.Register(
                new ResourceName(Domain, "model_data"),
                () => new ModelData()
            );
        }

        public static void Register(ExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry)
        {
            registry.Register((BuiltInEntityEvent.JoinedWorld _) => { });
            registry.Register((BuiltInEntityEvent.LeavingWorld _) => { });
        }
    }
}