using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class UniformBufferSet : IDisposable
    {
        private readonly NativeBufferPool _pool;
        private readonly Dictionary<IRenderLayer, LayerData> _layers = new();

        public UniformBufferSet(NativeBufferPool pool)
        {
            _pool = pool;
        }

        public void Setup(RenderContext context, CommandBufferRecorder cmd)
        {
            foreach (var (layer, data) in _layers)
            {
                layer.InitializeCommand(cmd);
                data.Uniforms?.Setup(context, cmd);
            }
        }

        public void AddAndUse(RenderContext context, CommandBufferRecorder cmd, IRenderLayer layer, Matrix4x4 transform)
        {
            if (!_layers.TryGetValue(layer, out var data))
            {
                _layers[layer] = data = new LayerData(layer.CreateUniforms(_pool));
                layer.InitializeCommand(cmd);
                data.Uniforms?.Setup(context, cmd);
            }

            data.Uniforms?.PushAndUseTransform(context, cmd, transform);
        }

        public void Finalize(RenderContext context, CommandBufferRecorder cmd)
        {
            foreach (var data in _layers.Values)
                data.Uniforms?.Finalize(context, cmd);
        }

        public void Clear()
        {
            foreach (var data in _layers.Values)
                data.Uniforms?.Clear();
        }

        public void Dispose()
        {
            foreach (var data in _layers.Values)
                data.Uniforms?.Dispose();
        }
        
        private sealed class LayerData
        {
            internal readonly IRenderLayerUniforms? Uniforms;

            public LayerData(IRenderLayerUniforms? uniforms)
            {
                Uniforms = uniforms;
            }
        }
    }
}