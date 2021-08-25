using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    public sealed class RegistryBuiltEvent<T> : IEvent where T : notnull
    {
        public Registry<T> Registry { get; }

        public RegistryBuiltEvent(Registry<T> registry)
        {
            Registry = registry;
        }
    }
}