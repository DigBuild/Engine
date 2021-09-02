using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigBuild.Engine.Ticking
{
    public sealed class Scheduler
    {
        private readonly Dictionary<ulong, ScheduledTick> _scheduledTicks = new();
        private ulong _now;

        public Scheduler(ITickSource tickSource)
        {
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
            
                return _scheduledTicks[_now + delay] = new ScheduledTick(this);
            }
        }

        private sealed class ScheduledTick : IScheduledTick
        {
            private readonly Scheduler _scheduler;
            private readonly Dictionary<IJobHandle, IScheduledJob> _scheduledJobs = new();

            public event Action? Tick;

            public ScheduledTick(Scheduler scheduler)
            {
                _scheduler = scheduler;
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
        public void Enqueue<TInput>(JobHandle<TInput> job, params TInput[] inputs)
        {
            Enqueue(job, (IEnumerable<TInput>) inputs);
        }

        public void Enqueue<TInput>(JobHandle<TInput> job, IEnumerable<TInput> inputs);
    }
}