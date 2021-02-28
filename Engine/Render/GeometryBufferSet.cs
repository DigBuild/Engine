using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuildPlatformCS.Render;
using DigBuildPlatformCS.Util;

namespace DigBuildEngine.Render
{
    public sealed class GeometryBufferSet : IDisposable
    {
        private readonly NativeBufferPool _pool;
        private readonly Dictionary<IWorldRenderLayer, ILayerData> _layers = new();
        private Matrix4x4 _transform = Matrix4x4.Identity;

        public GeometryBufferSet(NativeBufferPool pool)
        {
            _pool = pool;
        }

        public void SetTransform(Matrix4x4 transform)
        {
            _transform = transform;
        }

        public IVertexConsumer<TVertex> Get<TVertex>(WorldRenderLayer<TVertex> layer) where TVertex : unmanaged
        {
            if (!_layers.TryGetValue(layer, out var ld))
                _layers[layer] = ld = new LayerData<TVertex>(layer, _pool);
            var data = (LayerData<TVertex>) ld;

            data.Modified = true; // TODO: Maybe implement this a bit better
            
            var vertConsumer = new NativeBufferVertexConsumer<TVertex>(data.NativeBuffer);
            return layer.LinearTransformer(vertConsumer, _transform);
        }

        public void Draw(IWorldRenderLayer layer, RenderContext context, CommandBufferRecorder cmd)
        {
            if (_layers.TryGetValue(layer, out var data))
                data.Draw(context, cmd);
        }

        public void Clear()
        {
            foreach (var layerData in _layers.Values)
                layerData.Clear();
        }

        public void Dispose()
        {
            foreach (var layerData in _layers.Values)
                layerData.Dispose();
        }

        private interface ILayerData : IDisposable
        {
            void Draw(RenderContext context, CommandBufferRecorder cmd);
            void Clear();
        }

        private sealed class LayerData<TVertex> : ILayerData where TVertex : unmanaged
        {
            private readonly WorldRenderLayer<TVertex> _layer;
            internal readonly PooledNativeBuffer<TVertex> NativeBuffer;
            internal bool Modified;
            
            private VertexBuffer<TVertex>? _vertexBuffer;
            private VertexBufferWriter<TVertex>? _vertexBufferWriter;

            public LayerData(WorldRenderLayer<TVertex> layer, NativeBufferPool pool)
            {
                _layer = layer;
                NativeBuffer = pool.Request<TVertex>();
            }

            public void Draw(RenderContext context, CommandBufferRecorder cmd)
            {
                if (Modified)
                {
                    if (_vertexBuffer == null || _vertexBufferWriter == null)
                        _vertexBuffer = context.CreateVertexBuffer(out _vertexBufferWriter, NativeBuffer);
                    else
                        _vertexBufferWriter.Write(NativeBuffer);
                }

                cmd.Draw(_layer.Pipeline, _vertexBuffer!);
            }

            public void Clear()
            {
                NativeBuffer.Clear();
                Modified = true;
            }

            public void Dispose()
            {
                NativeBuffer.Dispose();
            }
        }
    }
}