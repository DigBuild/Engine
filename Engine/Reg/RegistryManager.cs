using System;
using System.Collections.Generic;
using DigBuildPlatformCS.Resource;

namespace DigBuildEngine.Reg
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