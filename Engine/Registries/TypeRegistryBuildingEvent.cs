using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    public sealed class TypeRegistryBuildingEvent<T, TValue> : IEvent
    {
        public TypeRegistryBuilder<T, TValue> Registry { get; }

        public TypeRegistryBuildingEvent(TypeRegistryBuilder<T, TValue> registry)
        {
            Registry = registry;
        }
    }
}