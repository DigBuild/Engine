using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Ticking
{
    /// <summary>
    /// A job handle.
    /// </summary>
    public interface IJob
    {
    }

    /// <summary>
    /// A job handle.
    /// </summary>
    /// <typeparam name="TInput">The input type</typeparam>
    public sealed class Job<TInput> : IJob
    {
        private readonly IJobExecutor<TInput> _executor;

        internal Job(IJobExecutor<TInput> executor)
        {
            _executor = executor;
        }

        internal Task Execute(Scheduler scheduler, IEnumerable<TInput> inputs)
        {
            return _executor.Execute(scheduler, inputs);
        }
    }

    /// <summary>
    /// Registry extensions for job handles.
    /// </summary>
    public static class JobHandleRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a job based on a user-provided executor.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The job name</param>
        /// <param name="executor">The executor</param>
        /// <returns>The job</returns>
        public static Job<TInput> Register<TInput>(this IRegistryBuilder<IJob> registry, ResourceName name, IJobExecutor<TInput> executor)
        {
            return registry.Add(name, new Job<TInput>(executor));
        }

        /// <summary>
        /// Registers a sequential job based on a user-provided delegate.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The job name</param>
        /// <param name="action">The delegate</param>
        /// <returns>The job</returns>
        public static Job<TInput> RegisterSequential<TInput>(this IRegistryBuilder<IJob> registry, ResourceName name, Action<Scheduler, TInput> action)
        {
            return registry.Register(name, JobExecutor.Sequential(action));
        }

        /// <summary>
        /// Registers a parallel job based on a user-provided delegate.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <param name="registry">The registry</param>
        /// <param name="name">The job name</param>
        /// <param name="action">The delegate</param>
        /// <returns>The job</returns>
        public static Job<TInput> RegisterParallel<TInput>(this IRegistryBuilder<IJob> registry, ResourceName name, Action<Scheduler, TInput> action)
        {
            return registry.Register(name, JobExecutor.Parallel(action));
        }
    }
}