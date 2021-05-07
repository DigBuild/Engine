using System;

namespace DigBuild.Engine.Particles
{
    public interface IParticleUpdateContext
    {
        Random Random { get; }
    }

    public sealed class ParticleUpdateContext : IParticleUpdateContext
    {
        public Random Random { get; } = new Random();
    }
}