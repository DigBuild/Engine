﻿using DigBuild.Engine.Math;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A read-only block context.
    /// </summary>
    public sealed class ReadOnlyBlockContext : IReadOnlyBlockContext
    {
        public IReadOnlyWorld World { get; }
        public BlockPos Pos { get; }
        public Block Block { get; }

        public ReadOnlyBlockContext(IReadOnlyWorld world, BlockPos pos, Block block)
        {
            World = world;
            Pos = pos;
            Block = block;
        }
    }
}