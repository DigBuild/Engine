using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    public sealed class RegistryBuildingEvent<T> : IEvent where T : notnull
    {
        public RegistryBuilder<T> Registry { get; }

        public RegistryBuildingEvent(RegistryBuilder<T> registry)
        {
            Registry = registry;
        }
    }
}