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

        public void AddGeometry(GeometryBufferSet buffers, IReadOnlyModelData data, Func<Direction, byte> light, DirectionFlags faces)
        {
        }

        public bool IsFaceSolid(Direction face) => true;
    }
}