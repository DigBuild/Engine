﻿using System;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render.Worlds
{
    public interface IWorldRenderer : IDisposable
    {
        void Update(RenderContext context, WorldView worldView, float partialTick);

        void BeforeDraw(RenderContext context, CommandBufferRecorder cmd, UniformBufferSet uniforms, WorldView worldView, float partialTick);
        void Draw(RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer, RenderLayerBindingSet bindings, IReadOnlyUniformBufferSet uniforms, WorldView worldView, float partialTick);
        void AfterDraw(RenderContext context, CommandBufferRecorder cmd, WorldView worldView, float partialTick);
    }
}