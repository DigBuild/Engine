using System;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render
{
    public interface IBlockModel
    {
        void AddGeometry(GeometryBufferSet buffers, IReadOnlyModelData data, Func<Direction, byte> light, DirectionFlags faces);

        bool IsFaceSolid(Direction face);

        bool HasDynamicGeometry => false;
        void AddDynamicGeometry(GeometryBufferSet buffers, IReadOnlyModelData data, Func<Direction, byte> light, float partialTick) { }
    }
}