﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Ticking
{
    public interface IJobHandle
    {
    }

    public sealed class JobHandle<TInput> : IJobHandle
    {
        private readonly IJob<TInput> _job;

        internal JobHandle(IJob<TInput> job)
        {
            _job = job;
        }

        internal Task Execute(Scheduler scheduler, IEnumerable<TInput> inputs)
        {
            return _job.Execute(scheduler, inputs);
        }
    }

    public static class JobHandleRegistryBuilderExtensions
    {
        public static JobHandle<TInput> Create<TInput>(this IRegistryBuilder<IJobHandle> registry, ResourceName name, IJob<TInput> job)
        {
            return registry.Add(name, new JobHandle<TInput>(job));
        }
        public static JobHandle<TInput> CreateSequential<TInput>(this IRegistryBuilder<IJobHandle> registry, ResourceName name, Action<Scheduler, TInput> action)
        {
            return registry.Create(name, Job.Sequential(action));
        }
        public static JobHandle<TInput> CreateParallel<TInput>(this IRegistryBuilder<IJobHandle> registry, ResourceName name, Action<Scheduler, TInput> action)
        {
            return registry.Create(name, Job.Parallel(action));
        }
    }
}