using System;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render
{
    public sealed class SolidBlockModel : IBlockModel
    {
        public static IBlockModel Instance { get; } = new SolidBlockModel();

        private SolidBlockModel()
        {
        }

        public void AddGeometry(DirectionFlags faces, GeometryBufferSet buffers, Func<Direction, byte> light)
        {
        }

        public bool IsFaceSolid(Direction face) => true;
    }
}