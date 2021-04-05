using System;
using System.Numerics;
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
    
    [Flags]
    public enum DirectionFlags : byte
    {
        None = 0,
        NegX = 1 << 0,
        PosX = 1 << 1,
        NegY = 1 << 2,
        PosY = 1 << 3,
        NegZ = 1 << 4,
        PosZ = 1 << 5,
        All = NegX | PosX | NegY | PosY | NegZ | PosZ
    }

    public static class BlockFaceFlagsExtensions
    {
        public static bool Has(this DirectionFlags flags, Direction face)
        {
            return (flags & face.ToFlags()) != DirectionFlags.None;
        }
    }
}