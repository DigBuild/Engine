using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuildPlatformCS.Render;
using DigBuildPlatformCS.Util;

namespace DigBuildEngine.Render
{
    public sealed class UniformBufferSet : IDisposable
    {
        private readonly NativeBufferPool _pool;
        private readonly Dictionary<IWorldRenderLayer, LayerData> _layers = new();

        public UniformBufferSet(NativeBufferPool pool)
        {
            _pool = pool;
        }

        public void Setup(RenderContext context, CommandBufferRecorder cmd)
        {
            foreach (var data in _layers.Values)
                data.Uniforms.Setup(context, cmd);
        }

        public void AddAndUse(RenderContext context, CommandBufferRecorder cmd, IWorldRenderLayer layer, Matrix4x4 transform)
        {
            if (!_layers.TryGetValue(layer, out var data))
            {
                _layers[layer] = data = new LayerData(layer.CreateUniforms(_pool));
                data.Uniforms.Setup(context, cmd);
            }

            data.Uniforms.PushAndUseTransform(context, cmd, transform);
        }

        public void Finalize(RenderContext context, CommandBufferRecorder cmd)
        {
            foreach (var data in _layers.Values)
                data.Uniforms.Finalize(context, cmd);
        }

        public void Clear()
        {
            foreach (var data in _layers.Values)
                data.Uniforms.Clear();
        }

        public void Dispose()
        {
            foreach (var data in _layers.Values)
                data.Uniforms.Dispose();
        }
        
        private sealed class LayerData
        {
            internal readonly IWorldRenderLayerUniforms Uniforms;

            public LayerData(IWorldRenderLayerUniforms uniforms)
            {
                Uniforms = uniforms;
            }
        }
    }
}