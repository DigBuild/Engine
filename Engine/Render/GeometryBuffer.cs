using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class GeometryBuffer : IGeometryBuffer, IDisposable
    {
        private readonly NativeBufferPool _bufferPool;

        private readonly Dictionary<IRenderLayer, IGeometry> _layers = new();

        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;
        public bool TransformNormal { get; set; } = true;

        public GeometryBuffer(NativeBufferPool bufferPool)
        {
            _bufferPool = bufferPool;
        }

        public IVertexConsumer<TVertex> Get<TVertex>(IRenderLayer<TVertex> layer)
            where TVertex : unmanaged
        {
            if (!_layers.TryGetValue(layer, out var g) || g is not Geometry<TVertex> geometry)
                _layers[layer] = geometry = new Geometry<TVertex>(layer, _bufferPool);

            return layer.CreateTransformer(geometry, Transform, TransformNormal);
        }

        public void Upload(RenderContext context)
        {
            foreach (var layerData in _layers.Values)
                layerData.Upload(context);
        }
        
        public void Draw(CommandBufferRecorder cmd, IRenderLayer layer, IReadOnlyUniformBufferSet uniforms)
        {
            if (_layers.TryGetValue(layer, out var data))
                data.Draw(cmd, uniforms);
        }

        public void Clear()
        {
            foreach (var layerData in _layers.Values)
                layerData.Clear();
        }

        public void Reset()
        {
            Clear();
            Transform = Matrix4x4.Identity;
            TransformNormal = true;
        }

        public void Dispose()
        {
            foreach (var layerData in _layers.Values)
                layerData.Dispose();
        }

        private interface IGeometry : IDisposable
        {
            void Upload(RenderContext context);
            void Draw(CommandBufferRecorder cmd, IReadOnlyUniformBufferSet uniforms);
            void Clear();
        }

        public class Geometry<TVertex> : IGeometry, IVertexConsumer<TVertex>
            where TVertex : unmanaged
        {
            private readonly IRenderLayer<TVertex> _layer;
            private readonly NativeBufferPool _bufferPool;
            private PooledNativeBuffer<TVertex>? _nativeBuffer;
            
            private VertexBuffer<TVertex>? _vertexBuffer;
            private VertexBufferWriter<TVertex>? _vertexBufferWriter;
            private uint _lastCount;

            private INativeBuffer<TVertex> NativeBuffer => _nativeBuffer ??= _bufferPool.Request<TVertex>();

            public Geometry(IRenderLayer<TVertex> layer, NativeBufferPool bufferPool)
            {
                _layer = layer;
                _bufferPool = bufferPool;
            }

            public void Accept(TVertex vertex)
            {
                NativeBuffer.Add(vertex);
            }

            public void Accept(IEnumerable<TVertex> vertices)
            {
                NativeBuffer.Add(vertices);
            }

            public void Accept(params TVertex[] vertices)
            {
                NativeBuffer.Add(vertices);
            }

            public void Upload(RenderContext context)
            {
                if (_nativeBuffer == null)
                    return;

                if (_vertexBuffer == null || _vertexBufferWriter == null)
                    _vertexBuffer = context.CreateVertexBuffer(out _vertexBufferWriter, _nativeBuffer);
                else
                    _vertexBufferWriter.Write(_nativeBuffer);

                _lastCount = _nativeBuffer.Count;
                _nativeBuffer.Dispose();
                _nativeBuffer = null;
            }
            
            public void Draw(CommandBufferRecorder cmd, IReadOnlyUniformBufferSet uniforms)
            {
                if (_lastCount > 0)
                    _layer.Draw(cmd, uniforms, _vertexBuffer!);
            }

            public void Clear()
            {
                _nativeBuffer?.Dispose();
                _nativeBuffer = null;
                _lastCount = 0;
            }

            public void Dispose()
            {
                Clear();
            }
        }
    }
}