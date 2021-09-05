using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigBuild.Engine.Ticking
{
    /// <summary>
    /// A deferred work scheduler.
    /// </summary>
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

        /// <summary>
        /// Gets the tick after a certain delay.
        /// </summary>
        /// <param name="delay">The delay</param>
        /// <returns>The tick</returns>
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
            private readonly Dictionary<IJob, IScheduledJob> _scheduledJobs = new();

            public event Action? Tick;

            public ScheduledTick(Scheduler scheduler)
            {
                _scheduler = scheduler;
            }

            public void Enqueue<TInput>(Job<TInput> job, IEnumerable<TInput> inputs)
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
                private readonly Job<TInput> _handle;
                private readonly ConcurrentQueue<TInput> _inputs = new();
                private bool _executing = false;

                public ScheduledJob(Job<TInput> handle)
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

    /// <summary>
    /// A deferred tick.
    /// </summary>
    public interface IScheduledTick : ITickSource
    {
        /// <summary>
        /// Enqueues a set of inputs for a job.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="job">The job</param>
        /// <param name="inputs">The inputs</param>
        public void Enqueue<TInput>(Job<TInput> job, params TInput[] inputs)
        {
            Enqueue(job, (IEnumerable<TInput>) inputs);
        }
        
        /// <summary>
        /// Enqueues a set of inputs for a job.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="job">The job</param>
        /// <param name="inputs">The inputs</param>
        public void Enqueue<TInput>(Job<TInput> job, IEnumerable<TInput> inputs);
    }
}