using System;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render
{
    public interface IBlockModel
    {
        void AddGeometry(DirectionFlags faces, GeometryBufferSet buffers, Func<Direction, byte> light);

        bool IsFaceSolid(Direction face);

        bool HasDynamicGeometry => false;
        void AddDynamicGeometry(GeometryBufferSet buffers, Func<Direction, byte> light) { }
    }
}