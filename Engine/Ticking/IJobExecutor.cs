using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigBuild.Engine.Ticking
{
    /// <summary>
    /// A job executor.
    /// </summary>
    /// <typeparam name="TInput">The job's input type</typeparam>
    public interface IJobExecutor<in TInput>
    {
        /// <summary>
        /// Executes the job on the given inputs and returns a task that completes when this is done.
        /// </summary>
        /// <param name="scheduler">The scheduler</param>
        /// <param name="inputs">The inputs</param>
        /// <returns>A completion task</returns>
        Task Execute(Scheduler scheduler, IEnumerable<TInput> inputs);
    }

    /// <summary>
    /// Job executor helpers.
    /// </summary>
    public static class JobExecutor
    {
        /// <summary>
        /// Creates a sequential executor from a delegate.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="action">The delegate</param>
        /// <returns>The executor</returns>
        public static IJobExecutor<TInput> Sequential<TInput>(Action<Scheduler, TInput> action)
        {
            return new SequentialExecutor<TInput>(action);
        }
        
        /// <summary>
        /// Creates a parallel executor from a delegate.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="action">The delegate</param>
        /// <returns>The executor</returns>
        public static IJobExecutor<TInput> Parallel<TInput>(Action<Scheduler, TInput> action)
        {
            return new ParallelExecutor<TInput>(action);
        }

        private sealed class SequentialExecutor<TInput> : IJobExecutor<TInput>
        {
            private readonly Action<Scheduler, TInput> _action;

            public SequentialExecutor(Action<Scheduler, TInput> action)
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

        private sealed class ParallelExecutor<TInput> : IJobExecutor<TInput>
        {
            private readonly Action<Scheduler, TInput> _action;

            public ParallelExecutor(Action<Scheduler, TInput> action)
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