﻿using DigBuild.Platform.Render;

namespace DigBuild.Engine.Render
{
    public interface IReadOnlyUniformBufferSet
    {
        UniformBuffer<TUniform> Get<TUniform>(RenderUniform<TUniform> uniform)
            where TUniform : unmanaged, IUniform<TUniform>;
        uint GetIndex<TUniform>(RenderUniform<TUniform> uniform)
            where TUniform : unmanaged, IUniform<TUniform>;
    }
}