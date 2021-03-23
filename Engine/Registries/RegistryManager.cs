using System;
using System.Collections.Generic;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Registries
{
    // Temporary
    public sealed class RegistryManager
    {
        private readonly List<IRegistryPrototype> _prototypes = new();
        
        public RegistryPrototype<T> CreateRegistryOf<T>(ResourceName name) where T : notnull
        {
            var prototype = new RegistryPrototype<T>();
            _prototypes.Add(prototype);
            return prototype;
        }

        public TypeRegistryPrototype<T> CreateRegistryOfTypes<T>(ResourceName name, Predicate<Type> typeValidator)
        {
            var prototype = new TypeRegistryPrototype<T>(typeValidator);
            _prototypes.Add(prototype);
            return prototype;
        }

        public ExtendedTypeRegistryPrototype<T, TValue> CreateExtendedRegistryOfTypes<T, TValue>(ResourceName name, Predicate<Type> typeValidator)
        {
            var prototype = new ExtendedTypeRegistryPrototype<T, TValue>(typeValidator);
            _prototypes.Add(prototype);
            return prototype;
        }

        public void BuildAll()
        {
            _prototypes.ForEach(p => p.Build());
        }
    }

    internal interface IRegistryPrototype {
        void Build();
    }

    public delegate void TypeRegistryBuildingEventHandler<T>(TypeRegistryBuilder<T> builder);
    public delegate void TypeRegistryBuiltEventHandler<T>(TypeRegistry<T> registry);
    public sealed class TypeRegistryPrototype<T> : IRegistryPrototype
    {
        private readonly Predicate<Type> _typeValidator;
        
        internal TypeRegistryPrototype(Predicate<Type> typeValidator)
        {
            _typeValidator = typeValidator;
        }

        public event TypeRegistryBuildingEventHandler<T>? Building;
        public event TypeRegistryBuiltEventHandler<T>? Built;

        public void Build()
        {
            var builder = new TypeRegistryBuilder<T>(_typeValidator);
            Building?.Invoke(builder);
            Built?.Invoke(new TypeRegistry<T>(builder));
        }
    }

    public delegate void ExtendedTypeRegistryBuildingEventHandler<T, TValue>(ExtendedTypeRegistryBuilder<T, TValue> builder);
    public delegate void ExtendedTypeRegistryBuiltEventHandler<T, TValue>(ExtendedTypeRegistry<T, TValue> registry);
    public sealed class ExtendedTypeRegistryPrototype<T, TValue> : IRegistryPrototype
    {
        private readonly Predicate<Type> _typeValidator;
        
        internal ExtendedTypeRegistryPrototype(Predicate<Type> typeValidator)
        {
            _typeValidator = typeValidator;
        }

        public event ExtendedTypeRegistryBuildingEventHandler<T, TValue>? Building;
        public event ExtendedTypeRegistryBuiltEventHandler<T, TValue>? Built;

        public void Build()
        {
            var builder = new ExtendedTypeRegistryBuilder<T, TValue>(_typeValidator);
            Building?.Invoke(builder);
            Built?.Invoke(new ExtendedTypeRegistry<T, TValue>(builder));
        }
    }

    public delegate void RegistryBuildingEventHandler<T>(RegistryBuilder<T> builder) where T : notnull;
    public delegate void RegistryBuiltEventHandler<T>(Registry<T> registry) where T : notnull;
    public sealed class RegistryPrototype<T> : IRegistryPrototype where T : notnull
    {
        internal RegistryPrototype()
        {
        }

        public event RegistryBuildingEventHandler<T>? Building;
        public event RegistryBuiltEventHandler<T>? Built;

        public void Build()
        {
            var builder = new RegistryBuilder<T>(null, null);
            Building?.Invoke(builder);
            Built?.Invoke(new Registry<T>(builder));
        }
    }
}