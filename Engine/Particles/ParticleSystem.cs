using System;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Particles
{
    public interface IParticleSystem<TGpu>
        where TGpu : unmanaged
    {
        internal INativeBuffer<TGpu> GpuBuffer { get; }
    }

    public sealed class ParticleSystem<T, TGpu> : IParticleSystem<TGpu>, IDisposable
        where T : unmanaged, IParticle<TGpu>
        where TGpu : unmanaged
    {
        private readonly PooledNativeBuffer<T> _instanceBuffer;
        private readonly PooledNativeBuffer<TGpu> _gpuBuffer;

        INativeBuffer<TGpu> IParticleSystem<TGpu>.GpuBuffer => _gpuBuffer;

        public ParticleSystem(NativeBufferPool pool)
        {
            _instanceBuffer = pool.Request<T>();
            _gpuBuffer = pool.Request<TGpu>();
        }

        public void Dispose()
        {
            _instanceBuffer.Dispose();
            _gpuBuffer.Dispose();
        }

        public Span<T> Create(uint amount)
        {
            var span = _instanceBuffer.Add(amount);
            _gpuBuffer.Add(amount);
            return span;
        }

        public void Update()
        {
            var bottom = 0u;
            var top = _instanceBuffer.Count - 1;

            for (; bottom <= top; bottom++)
            {
                if (_instanceBuffer[bottom].Update(ref _gpuBuffer[bottom]))
                    continue;

                for (; bottom < top; top--)
                {
                    if (!_instanceBuffer[top].Update(ref _gpuBuffer[top]))
                        continue;

                    _instanceBuffer[bottom] = _instanceBuffer[top];
                    _gpuBuffer[bottom] = _gpuBuffer[top];
                    break;
                }
            }
        }
    }
}