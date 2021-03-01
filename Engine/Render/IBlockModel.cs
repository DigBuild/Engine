using System;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Render
{
    public interface IBlockModel
    {
        void AddGeometry(BlockFaceFlags faces, GeometryBufferSet buffers);
    }
    
    [Flags]
    public enum BlockFaceFlags : byte
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
        public static bool HasFace(this BlockFaceFlags flags, BlockFace face)
        {
            return (flags & face.ToFlags()) != BlockFaceFlags.None;
        }
    }
}