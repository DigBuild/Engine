using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// Fired when a registry's contents are locked.
    /// </summary>
    /// <typeparam name="T">The registry type</typeparam>
    public sealed class RegistryBuiltEvent<T> : IEvent where T : notnull
    {
        /// <summary>
        /// The registry.
        /// </summary>
        public Registry<T> Registry { get; }

        public RegistryBuiltEvent(Registry<T> registry)
        {
            Registry = registry;
        }
    }
}