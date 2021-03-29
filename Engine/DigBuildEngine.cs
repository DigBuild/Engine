using DigBuild.Engine.Blocks;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Registries;
using DigBuild.Engine.Worlds;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine
{
    public static class DigBuildEngine
    {
        public const string Domain = "digbuildengine";

        public static void Register(RegistryBuilder<IWorldStorageType> registry)
        {
            EntityWorldStorage.Type = registry.Create<IReadOnlyEntityWorldStorage, EntityWorldStorage>(
                new ResourceName(Domain, "entities")
            );
        }

        public static void Register(RegistryBuilder<IChunkStorageType> registry)
        {
            BlockChunkStorage.Type = registry.Create<IReadOnlyBlockChunkStorage, BlockChunkStorage>(
                new ResourceName(Domain, "blocks")
            );
        }

        public static void Register(ExtendedTypeRegistryBuilder<IBlockEvent, BlockEventInfo> registry)
        {
            registry.Register((IBlockContext context, BuiltInBlockEvent.JoinedWorld evt) => { });
            registry.Register((IBlockContext context, BuiltInBlockEvent.LeavingWorld evt) => { });
        }

        public static void Register(ExtendedTypeRegistryBuilder<IEntityEvent, EntityEventInfo> registry)
        {
            registry.Register((IEntityContext context, BuiltInEntityEvent.JoinedWorld evt) => { });
            registry.Register((IEntityContext context, BuiltInEntityEvent.LeavingWorld evt) => { });
        }
    }
}