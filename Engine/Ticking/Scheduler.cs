using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (_scheduledTicks.TryGetValue(_now, out var tick))
            {
                tick.DoTick();
                _scheduledTicks.Remove(_now);
            }
            _now++;
        }

        public IScheduledTick After(ulong delay)
        {
            lock (_scheduledTicks)
            {
                if (_scheduledTicks.TryGetValue(_now + delay, out var tick))
                    return tick;
            
                return _scheduledTicks[_now + delay] = new ScheduledTick(this, _now, delay);
            }
        }

        private sealed class ScheduledTick : IScheduledTick, IInterpolator
        {
            private readonly Scheduler _scheduler;
            private readonly ulong _startTime, _delay;
            private readonly Dictionary<IJobHandle, IScheduledJob> _scheduledJobs = new();

            public event Action? Tick;
            public IInterpolator Interpolator => this;

            public float Value => (float) System.Math.Min((_scheduler._now + _scheduler._tickSource.CurrentTick.Value - _startTime) / (double) _delay, 1);

            public ScheduledTick(Scheduler scheduler, ulong startTime, ulong delay)
            {
                _scheduler = scheduler;
                _startTime = startTime;
                _delay = delay;
            }

            public void Enqueue<TInput>(JobHandle<TInput> job, IEnumerable<TInput> inputs)
            {
                IScheduledJob? scheduledJob;

                lock (_scheduledJobs)
                {
                    if (!_scheduledJobs.TryGetValue(job, out scheduledJob))
                        _scheduledJobs[job] = scheduledJob = new ScheduledJob<TInput>(job);
                }

                ((ScheduledJob<TInput>) scheduledJob).Enqueue(inputs);
            }

            internal void DoTick()
            {
                Tick?.Invoke();
                foreach (var job in _scheduledJobs.Values)
                    job.Execute(_scheduler).Wait();
            }

            private interface IScheduledJob
            {
                Task Execute(Scheduler scheduler);
            }

            private sealed class ScheduledJob<TInput> : IScheduledJob
            {
                private readonly JobHandle<TInput> _handle;
                private readonly ConcurrentQueue<TInput> _inputs = new();
                private bool _executing = false;

                public ScheduledJob(JobHandle<TInput> handle)
                {
                    _handle = handle;
                }

                public void Enqueue(IEnumerable<TInput> inputs)
                {
                    if (_executing)
                        throw new Exception("Cannot enqueue inputs on the currently executing job.");

                    foreach (var input in inputs)
                        _inputs.Enqueue(input);
                }

                public Task Execute(Scheduler scheduler)
                {
                    _executing = true;
                    return _handle.Execute(scheduler, _inputs);
                }
            }
        }
    }

    public interface IScheduledTick : ITickSource
    {
        public IInterpolator Interpolator { get; }

        public void Enqueue<TInput>(JobHandle<TInput> job, params TInput[] inputs)
        {
            Enqueue(job, (IEnumerable<TInput>) inputs);
        }

        public void Enqueue<TInput>(JobHandle<TInput> job, IEnumerable<TInput> inputs);
    }
}