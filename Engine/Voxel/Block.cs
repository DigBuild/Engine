using System;
using DigBuildEngine.Math;
using DigBuildEngine.Reg;
using DigBuildEngine.Util;
using DigBuildPlatformCS.Resource;

namespace DigBuildEngine.Voxel
{
    public sealed class Block
    {
        // public static Block CreateTmp(uint id, AABB boundingBox) => new(id, new VoxelCollider(boundingBox));
        // public static Block CreateTmp(uint id, ICollider collider) => new(id, collider);
        // public static Block CreateRegistryOf(params Action<BlockBuilder>[] buildActions) => throw new NotImplementedException();

        public readonly uint Id;
        public readonly ICollider Collider;

        internal Block(uint id, ICollider collider)
        {
            Id = id;
            Collider = collider;
        }
    }

    public static class BlockRegistryBuilderExtensions
    {
        public static Block Create(this RegistryBuilder<Block> builder, ResourceName name,
            params Action<BlockBuilder>[] buildActions)
        {
            throw new NotImplementedException();
        }

        public static Block CreateTmp(this RegistryBuilder<Block> builder, ResourceName name,
            uint id, ICollider collider)
        {
            var block = new Block(id, collider);
            ((IRegistryBuilder<Block>)builder).Add(name, block);
            return block;
        }
    }
}