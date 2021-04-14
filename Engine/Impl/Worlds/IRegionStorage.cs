using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Math;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public interface IRegionStorage
    {
        bool TryLoad(RegionChunkPos pos, [NotNullWhen(true)] out Chunk? chunk);

        void Save(Chunk chunk);

        DataContainer<IRegion> LoadOrCreateManagedData();

        ILowDensityRegion LoadOrCreateManagedLowDensity();
    }
}