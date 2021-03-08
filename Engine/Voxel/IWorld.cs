﻿using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public interface IWorld : IReadOnlyWorld
    {
        // public new T Get<T>() where T : IWorldStorage;

        public new IChunk? GetChunk(ChunkPos pos, bool load = true);

        IReadOnlyChunk? IReadOnlyWorld.GetChunk(ChunkPos pos, bool load) => GetChunk(pos, load);

        public void OnBlockChanged(BlockPos pos);
    }

    public interface IWorldStorage : IReadOnlyWorldStorage
    {
    }
    public interface IWorldStorage<out T> : IWorldStorage where T : class, IWorldStorage<T>, new()
    {
    }
}