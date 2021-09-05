using System;
using System.Collections.Generic;
using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// A registry instantiator and initializer.
    /// </summary>
    public sealed class RegistryManager
    {
        private readonly List<Action> _builders = new();

        private readonly EventBus _eventBus;

        public RegistryManager(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// Creates a new registry.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="name">The registry name</param>
        /// <param name="nameValidator">The entry name validator</param>
        /// <param name="valueValidator">The entry value validator</param>
        public void CreateRegistryOf<T>(
            ResourceName name,
            Predicate<ResourceName>? nameValidator = null,
            Predicate<T>? valueValidator = null
        ) where T : notnull
        {
            _builders.Add(() =>
            {
                var builder = new RegistryBuilder<T>(name, nameValidator, valueValidator);
                _eventBus.Post(new RegistryBuildingEvent<T>(builder));

                var registry = new Registry<T>(name, builder);
                _eventBus.Post(new RegistryBuiltEvent<T>(registry));
            });
        }

        /// <summary>
        /// Creates a new registry.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="domain">The registry domain</param>
        /// <param name="path">The registry path</param>
        /// <param name="nameValidator">The entry name validator</param>
        /// <param name="valueValidator">The entry value validator</param>
        public void CreateRegistryOf<T>(
            string domain, string path,
            Predicate<ResourceName>? nameValidator = null,
            Predicate<T>? valueValidator = null
        ) where T : notnull
        {
            CreateRegistryOf(new ResourceName(domain, path), nameValidator, valueValidator);
        }
        
        /// <summary>
        /// Creates a new type registry.
        /// </summary>
        /// <typeparam name="T">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="name">The registry name</param>
        /// <param name="typeValidator">The entry type validator</param>
        /// <param name="valueValidator">The entry value validator</param>
        public void CreateTypeRegistryOf<T, TValue>(
            ResourceName name,
            Predicate<Type>? typeValidator = null,
            Predicate<TValue>? valueValidator = null
        )
        {
            _builders.Add(() =>
            {
                var builder = new TypeRegistryBuilder<T, TValue>(name, typeValidator, valueValidator);
                _eventBus.Post(new TypeRegistryBuildingEvent<T, TValue>(builder));
            
                var registry = new TypeRegistry<T, TValue>(name, builder);
                _eventBus.Post(new TypeRegistryBuiltEvent<T, TValue>(registry));
            });
        }
        
        /// <summary>
        /// Creates a new type registry.
        /// </summary>
        /// <typeparam name="T">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="domain">The registry domain</param>
        /// <param name="path">The registry path</param>
        /// <param name="typeValidator">The entry type validator</param>
        /// <param name="valueValidator">The entry value validator</param>
        public void CreateTypeRegistryOf<T, TValue>(
            string domain, string path,
            Predicate<Type>? typeValidator = null,
            Predicate<TValue>? valueValidator = null
        ) where T : notnull
        {
            CreateTypeRegistryOf<T, TValue>(new ResourceName(domain, path), typeValidator, valueValidator);
        }

        /// <summary>
        /// Builds all the registries, firing events as necessary.
        /// </summary>
        public void BuildAll()
        {
            _builders.ForEach(build => build());
        }
    }
}