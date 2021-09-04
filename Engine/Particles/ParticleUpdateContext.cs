using System;
using System.Numerics;

namespace DigBuild.Engine.Particles
{
    /// <summary>
    /// A particle update context.
    /// </summary>
    public interface IParticleUpdateContext
    {
        /// <summary>
        /// A source of random numbers.
        /// </summary>
        Random Random { get; }

        /// <summary>
        /// Gets the wind speed at a certain position.
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The wind speed</returns>
        Vector3 GetWindAt(Vector3 position);
    }

    /// <summary>
    /// A particle update context.
    /// </summary>
    public sealed class ParticleUpdateContext : IParticleUpdateContext
    {
        public Random Random { get; } = new();

        /// <summary>
        /// The global wind speed.
        /// </summary>
        public Vector3 Wind { get; set; }

        public Vector3 GetWindAt(Vector3 position) => Wind;
    }
}