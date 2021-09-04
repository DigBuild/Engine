using System;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Particles
{
    /// <summary>
    /// A particle system.
    /// </summary>
    public interface IParticleSystem
    {
        /// <summary>
        /// Updates the particle system with the given context.
        /// </summary>
        /// <param name="context">The context</param>
        void Update(IParticleUpdateContext context);
    }

    /// <summary>
    /// A particle system.
    /// </summary>
    /// <typeparam name="TGpu">The GPU particle type</typeparam>
    public interface IParticleSystem<TGpu> : IParticleSystem
        where TGpu : unmanaged
    {
        internal INativeBuffer<TGpu> UpdateGpu(float partialTick);
    }

    /// <summary>
    /// A particle system.
    /// </summary>
    /// <typeparam name="T">The particle type</typeparam>
    /// <typeparam name="TGpu">The GPU particle type</typeparam>
    public sealed class ParticleSystem<T, TGpu> : IParticleSystem<TGpu>, IDisposable
        where T : unmanaged, IParticle<TGpu>
        where TGpu : unmanaged
    {
        private readonly PooledNativeBuffer<T> _instanceBuffer;
        private readonly PooledNativeBuffer<TGpu> _gpuBuffer;

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

        /// <summary>
        /// Creates a new set of particles and returns a span containing them.
        /// </summary>
        /// <param name="amount">The amount of particles</param>
        /// <returns>The span with the particles</returns>
        public Span<T> Create(uint amount)
        {
            var span = _instanceBuffer.Add(amount);
            _gpuBuffer.Add(amount);
            return span;
        }

        public void Update(IParticleUpdateContext context)
        {
            if (_instanceBuffer.Count == 0)
                return;

            var bottom = 0u;
            var top = _instanceBuffer.Count - 1;
            
            for (; bottom <= top; bottom++)
            {
                if (_instanceBuffer[bottom].Update(context))
                    continue;

                for (; bottom < top; top--)
                {
                    if (!_instanceBuffer[top].Update(context))
                        continue;

                    _instanceBuffer[bottom] = _instanceBuffer[top];
                    _gpuBuffer[bottom] = _gpuBuffer[top];
                    top--;
                    break;
                }
            }

            var removed = (_instanceBuffer.Count - 1) - top;
            if (removed <= 0)
                return;

            _instanceBuffer.ReleaseLast(removed, true);
            _gpuBuffer.ReleaseLast(removed, true);
        }

        INativeBuffer<TGpu> IParticleSystem<TGpu>.UpdateGpu(float partialTick)
        {
            var count = _instanceBuffer.Count;

            for (var i = 0u; i < count; i++)
                _instanceBuffer[i].UpdateGpu(ref _gpuBuffer[i], partialTick);

            return _gpuBuffer;
        }
    }
}