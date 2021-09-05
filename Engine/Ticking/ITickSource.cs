using System;

namespace DigBuild.Engine.Ticking
{
    /// <summary>
    /// A source of tick events.
    /// </summary>
    public interface ITickSource
    {
        /// <summary>
        /// Fired every tick.
        /// </summary>
        event Action Tick;
    }

    /// <summary>
    /// A source of tick events capable of interpolation.
    /// </summary>
    public interface IStableTickSource : ITickSource
    {
        /// <summary>
        /// An interpolator for the current tick.
        /// </summary>
        IInterpolator CurrentTick { get; }
    }
}