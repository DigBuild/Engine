using DigBuild.Engine.Math;

namespace DigBuild.Engine.Voxel
{
    public interface IReadOnlyWorld
    {
        public T Get<T>() where T : class, IWorldStorage<T>, new();

        IReadOnlyChunk? GetChunk(ChunkPos pos, bool load = true);
    }

    public interface IReadOnlyWorldStorage
    {
    }
}