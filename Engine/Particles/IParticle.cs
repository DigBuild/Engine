namespace DigBuild.Engine.Particles
{
    public interface IParticle<TGpuParticle>
        where TGpuParticle : unmanaged
    {
        bool Update(ref TGpuParticle gpu);
    }
}