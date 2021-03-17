using System;

namespace DigBuild.Engine.Ticking
{
    public interface ITickSource
    {
        event Action Tick;
    }

    public interface IStableTickSource : ITickSource
    {
        IInterpolator CurrentTick { get; }
    }
}