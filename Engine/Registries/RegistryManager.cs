using System;
using System.Collections.Generic;
using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    public sealed class RegistryManager
    {
        private readonly List<Action> _builders = new();

        private readonly EventBus _eventBus;

        public RegistryManager(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

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

        public void CreateRegistryOf<T>(
            string domain, string path,
            Predicate<ResourceName>? nameValidator = null,
            Predicate<T>? valueValidator = null
        ) where T : notnull
        {
            CreateRegistryOf(new ResourceName(domain, path), nameValidator, valueValidator);
        }
        
        public void CreateTypeRegistryOf<T, TValue>(
            ResourceName name,
            Predicate<Type>? typeValidator = null,
            Predicate<TValue>? valueValidator = null
        ) where T : notnull
        {
            _builders.Add(() =>
            {
                var builder = new TypeRegistryBuilder<T, TValue>(name, typeValidator, valueValidator);
                _eventBus.Post(new TypeRegistryBuildingEvent<T, TValue>(builder));
            
                var registry = new TypeRegistry<T, TValue>(name, builder);
                _eventBus.Post(new TypeRegistryBuiltEvent<T, TValue>(registry));
            });
        }

        public void CreateTypeRegistryOf<T, TValue>(
            string domain, string path,
            Predicate<Type>? typeValidator = null,
            Predicate<TValue>? valueValidator = null
        ) where T : notnull
        {
            CreateTypeRegistryOf<T, TValue>(new ResourceName(domain, path), typeValidator, valueValidator);
        }

        public void BuildAll()
        {
            _builders.ForEach(build => build());
        }
    }
}