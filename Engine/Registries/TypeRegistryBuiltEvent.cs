using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    public sealed class TypeRegistryBuiltEvent<T, TValue> : IEvent where T : notnull
    {
        public TypeRegistry<T, TValue> Registry { get; }

        public TypeRegistryBuiltEvent(TypeRegistry<T, TValue> registry)
        {
            Registry = registry;
        }
    }
}