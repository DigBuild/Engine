using DigBuild.Engine.Blocks;

namespace DigBuild.Engine.Voxel
{
    public interface IBlockChunkStorage : IChunkStorage
    {
        public IBlockContainer Blocks { get; }
        public IBlockDataContainerContainer Data { get; }

        public void CopyFrom(IBlockChunkStorage other);
    }

    public interface IBlockContainer
    {
        public Block? this[int x, int y, int z] { get; set; }
    }

    public interface IBlockDataContainerContainer
    {
        public BlockDataContainer? this[int x, int y, int z] { get; set; }
    }
}