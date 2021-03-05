﻿using System.Numerics;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Render
{
    public interface ICamera
    {
        Matrix4x4 Transform { get; }
        Vector3 Position { get; }

        Matrix4x4 GetInterpolatedTransform(float partialTick);
        public RayCaster.Ray GetInterpolatedRay(float partialTick);
    }
}