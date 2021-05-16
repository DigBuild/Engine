using System;
using System.Collections.Generic;
using DigBuild.Platform.Render;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class UniformBufferSet : IUniformBufferSetWriter, IReadOnlyUniformBufferSet
    {
        private readonly NativeBufferPool _bufferPool;

        private readonly Dictionary<IRenderUniform, IData> _uniforms = new();
        
        public UniformBufferSet(IEnumerable<IRenderUniform> uniforms, NativeBufferPool bufferPool)
        {
            _bufferPool = bufferPool;
            foreach (var uniform in uniforms)
                _uniforms[uniform] = uniform.CreateData(bufferPool);
        }

        private Data<TUniform> GetData<TUniform>(RenderUniform<TUniform> uniform)
            where TUniform : unmanaged, IUniform<TUniform>
        {
            if (!_uniforms.TryGetValue(uniform, out var d) || d is not Data<TUniform> data)
                _uniforms[uniform] = data = new Data<TUniform>(_bufferPool);
            return data;
        }

        public uint Push<TUniform>(RenderUniform<TUniform> uniform, TUniform value)
            where TUniform : unmanaged, IUniform<TUniform>
        {
            var data = GetData(uniform);
            data.Push(value);
            return data.Index;
        }

        public UniformBuffer<TUniform> Get<TUniform>(RenderUniform<TUniform> uniform)
            where TUniform : unmanaged, IUniform<TUniform>
        {
            return GetData(uniform).Buffer;
        }

        public uint GetIndex<TUniform>(RenderUniform<TUniform> uniform)
            where TUniform : unmanaged, IUniform<TUniform>
        {
            return GetData(uniform).Index;
        }

        public Snapshot CaptureSnapshot()
        {
            return new(this);
        }

        public void Setup(RenderContext context)
        {
            foreach (var data in _uniforms.Values)
                data.Setup(context);
        }

        public void Upload(RenderContext context)
        {
            foreach (var data in _uniforms.Values)
                data.Upload(context);
        }

        public void Clear()
        {
            foreach (var data in _uniforms.Values)
                data.Clear();
        }

        public void Dispose()
        {
            foreach (var data in _uniforms.Values)
                data.Dispose();
        }

        internal interface IData : IDisposable
        {
            uint Index { get; }

            void Setup(RenderContext context);
            void Upload(RenderContext context);
            void Clear();
        }

        internal sealed class Data<TUniform> : IData
            where TUniform : unmanaged, IUniform<TUniform>
        {
            private readonly NativeBufferPool _bufferPool;
            private PooledNativeBuffer<TUniform>? _nativeBuffer;
            
            private UniformBuffer<TUniform>? _uniformBuffer;

            private INativeBuffer<TUniform> NativeBuffer => _nativeBuffer ??= _bufferPool.Request<TUniform>();

            public UniformBuffer<TUniform> Buffer => _uniformBuffer!;
            public uint Index => (_nativeBuffer?.Count ?? 0) - 1;

            public Data(NativeBufferPool bufferPool)
            {
                _bufferPool = bufferPool;
            }

            public void Push(TUniform value)
            {
                NativeBuffer.Add(value);
            }

            public void Setup(RenderContext context)
            {
                _uniformBuffer ??= context.CreateUniformBuffer<TUniform>();
            }

            public void Upload(RenderContext context)
            {
                if (_nativeBuffer == null)
                    return;
                
                _uniformBuffer!.Write(_nativeBuffer);
                
                _nativeBuffer.Dispose();
                _nativeBuffer = null;
            }

            public void Clear()
            {
                _nativeBuffer?.Dispose();
                _nativeBuffer = null;
            }

            public void Dispose()
            {
                Clear();
            }
        }

        public sealed class Snapshot : IReadOnlyUniformBufferSet
        {
            private readonly UniformBufferSet _uniforms;
            private readonly Dictionary<IRenderUniform, uint> _indices = new();
            
            public Snapshot(UniformBufferSet uniforms)
            {
                _uniforms = uniforms;
                foreach (var (key, data) in uniforms._uniforms)
                    _indices[key] = data.Index;
            }

            public UniformBuffer<TUniform> Get<TUniform>(RenderUniform<TUniform> uniform) where TUniform : unmanaged, IUniform<TUniform>
            {
                return _uniforms.Get(uniform);
            }

            public uint GetIndex<TUniform>(RenderUniform<TUniform> uniform) where TUniform : unmanaged, IUniform<TUniform>
            {
                return _indices[uniform];
            }
        }
    }
}