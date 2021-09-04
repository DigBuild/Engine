namespace DigBuild.Engine.Particles
{
    /// <summary>
    /// A particle.
    /// </summary>
    /// <typeparam name="TGpuParticle">A GPU representation of the same particle</typeparam>
    public interface IParticle<TGpuParticle>
        where TGpuParticle : unmanaged
    {
        /// <summary>
        /// Updates this particle using the given context.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>Whether the particle is still alive after this update</returns>
        bool Update(IParticleUpdateContext context);

        /// <summary>
        /// Updates the GPU representation of this particle.
        /// </summary>
        /// <param name="gpu">The GPU particle</param>
        /// <param name="partialTick">The partial tick for interpolation</param>
        void UpdateGpu(ref TGpuParticle gpu, float partialTick);
    }
}