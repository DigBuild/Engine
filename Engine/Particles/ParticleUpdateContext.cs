using System;
using System.Numerics;

namespace DigBuild.Engine.Particles
{
    public interface IParticleUpdateContext
    {
        Random Random { get; }

        Vector3 GetWindAt(Vector3 position);
    }

    public sealed class ParticleUpdateContext : IParticleUpdateContext
    {
        public Random Random { get; } = new();

        public Vector3 Wind { get; set; }

        public Vector3 GetWindAt(Vector3 position) => Wind;
    }
}