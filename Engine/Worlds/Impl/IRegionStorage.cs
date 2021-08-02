using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
{
    public interface IRegionStorage
    {
        bool TryLoad(RegionChunkPos pos, [NotNullWhen(true)] out Chunk? chunk);

        void Save(Chunk chunk);

        DataContainer<IRegion> LoadOrCreateManagedData();
    }
}