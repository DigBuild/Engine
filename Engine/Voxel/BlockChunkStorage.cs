using System;

namespace DigBuildEngine.Voxel
{
    public class BlockChunkStorage : IChunkStorage
    {
        public BlockContainer Blocks { get; }

        public BlockChunkStorage(Action notifyUpdate)
        {
            Blocks = new BlockContainer(notifyUpdate);
        }

        public sealed class BlockContainer
        {
            private readonly Action _notifyUpdate;
            private readonly Block?[,,] _blocks = new Block[16, 16, 16];

            public BlockContainer(Action notifyUpdate)
            {
                _notifyUpdate = notifyUpdate;
            }

            public Block? this[int x, int y, int z]
            {
                get => _blocks[x, y, z];
                set
                {
                    _blocks[x, y, z] = value;
                    _notifyUpdate();
                }
            }
        }
    }
}