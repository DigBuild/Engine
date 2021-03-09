using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class GeometryBufferSet : IDisposable
    {
        private readonly NativeBufferPool _pool;
        private readonly Dictionary<IRenderLayer, ILayerData> _layers = new();

        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

        public GeometryBufferSet(NativeBufferPool pool)
        {
            _pool = pool;
        }

        public IVertexConsumer<TVertex> Get<TVertex>(RenderLayer<TVertex> layer) where TVertex : unmanaged
        {
            if (!_layers.TryGetValue(layer, out var ld))
                _layers[layer] = ld = new LayerData<TVertex>(layer, _pool);
            var data = (LayerData<TVertex>) ld;
            
            var vertConsumer = new NativeBufferVertexConsumer<TVertex>(data.NativeBuffer);
            return layer.LinearTransformer(vertConsumer, Transform);
        }

        public void Draw(IRenderLayer layer, RenderContext context, CommandBufferRecorder cmd)
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
            private readonly RenderLayer<TVertex> _layer;
            private readonly NativeBufferPool _pool;
            private PooledNativeBuffer<TVertex>? _nativeBuffer;
            
            private VertexBuffer<TVertex>? _vertexBuffer;
            private VertexBufferWriter<TVertex>? _vertexBufferWriter;

            internal INativeBuffer<TVertex> NativeBuffer
            {
                get
                {
                    if (_nativeBuffer != null)
                        return _nativeBuffer;
                    return _nativeBuffer = _pool.Request<TVertex>();
                }
            }

            public LayerData(RenderLayer<TVertex> layer, NativeBufferPool pool)
            {
                _layer = layer;
                _pool = pool;
            }

            public void Draw(RenderContext context, CommandBufferRecorder cmd)
            {
                if (_nativeBuffer != null)
                {
                    if (_vertexBuffer == null || _vertexBufferWriter == null)
                        _vertexBuffer = context.CreateVertexBuffer(out _vertexBufferWriter, _nativeBuffer);
                    else
                        _vertexBufferWriter.Write(_nativeBuffer);
                    _nativeBuffer.Dispose();
                    _nativeBuffer = null;
                }

                _layer.Draw(cmd, _vertexBuffer!);
            }

            public void Clear()
            {
                NativeBuffer.Clear();
            }

            public void Dispose()
            {
                _nativeBuffer?.Dispose();
            }
        }
    }
}