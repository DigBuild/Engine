using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    public sealed class TypeRegistryBuildingEvent<T, TValue> : IEvent where T : notnull
    {
        public TypeRegistryBuilder<T, TValue> Registry { get; }

        public TypeRegistryBuildingEvent(TypeRegistryBuilder<T, TValue> registry)
        {
            Registry = registry;
        }
    }
}