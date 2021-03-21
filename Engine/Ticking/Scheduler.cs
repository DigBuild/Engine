using System;
using System.Collections.Generic;

namespace DigBuild.Engine.Ticking
{
    public sealed class Scheduler
    {
        private readonly IStableTickSource _tickSource;
        private readonly Dictionary<ulong, ScheduledTick> _scheduledTicks = new();
        private ulong _now = 0;

        public Scheduler(IStableTickSource tickSource)
        {
            _tickSource = tickSource;
            tickSource.Tick += Tick;
        }

        private void Tick()
        {
            if (_scheduledTicks.Remove(_now, out var tick))
                tick.DoTick();
            _now++;
        }

        public ScheduledTick Schedule(ulong delay)
        {
            if (_scheduledTicks.TryGetValue(_now + delay, out var tick))
                return tick;
            
            return _scheduledTicks[_now + delay] = new ScheduledTick(new Interpolator(this, _now, delay));
        }

        private readonly struct Interpolator : IInterpolator
        {
            private readonly Scheduler _scheduler;
            private readonly ulong _startTime, _delay;

            public float Value => (float) System.Math.Min((_scheduler._now + _scheduler._tickSource.CurrentTick.Value - _startTime) / (double) _delay, 1);
            
            public Interpolator(Scheduler scheduler, ulong startTime, ulong delay)
            {
                _scheduler = scheduler;
                _startTime = startTime;
                _delay = delay;
            }
        }
    }

    public sealed class ScheduledTick : ITickSource
    {
        public event Action? Tick;
        public IInterpolator Interpolator { get; }

        public ScheduledTick(IInterpolator interpolator)
        {
            Interpolator = interpolator;
        }

        internal void DoTick()
        {
            Tick?.Invoke();
        }
    }
}