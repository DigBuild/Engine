using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DigBuild.Engine.Ticking
{
    public interface IJob<in TInput>
    {
        public Task Execute(Scheduler scheduler, [ReadOnly(true)] IEnumerable<TInput> inputs);
    }

    public static class Job
    {
        public static IJob<TInput> Sequential<TInput>(Action<Scheduler, TInput> action)
        {
            return new SequentialJob<TInput>(action);
        }
        
        public static IJob<TInput> Parallel<TInput>(Action<Scheduler, TInput> action)
        {
            return new ParallelJob<TInput>(action);
        }

        private sealed class SequentialJob<TInput> : IJob<TInput>
        {
            private readonly Action<Scheduler, TInput> _action;

            public SequentialJob(Action<Scheduler, TInput> action)
            {
                _action = action;
            }

            public Task Execute(Scheduler scheduler, IEnumerable<TInput> inputs)
            {
                return Task.Run(() =>
                {
                    foreach (var input in inputs)
                        _action(scheduler, input);
                });
            }
        }

        private sealed class ParallelJob<TInput> : IJob<TInput>
        {
            private readonly Action<Scheduler, TInput> _action;

            public ParallelJob(Action<Scheduler, TInput> action)
            {
                _action = action;
            }
            
            public Task Execute(Scheduler scheduler, IEnumerable<TInput> inputs)
            {
                return Task.Run(() => System.Threading.Tasks.Parallel.ForEach(inputs, input => _action(scheduler, input)));
            }
        }
    }
}