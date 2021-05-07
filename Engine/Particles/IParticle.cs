namespace DigBuild.Engine.Particles
{
    public interface IParticle<TGpuParticle>
        where TGpuParticle : unmanaged
    {
        bool Update(IParticleUpdateContext context);

        void UpdateGpu(ref TGpuParticle gpu, float partialTick);
    }
}