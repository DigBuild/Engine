using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// Fired when a registry is being built so new entries can be added.
    /// </summary>
    /// <typeparam name="T">The registry type</typeparam>
    public sealed class RegistryBuildingEvent<T> : IEvent where T : notnull
    {
        /// <summary>
        /// The registry builder.
        /// </summary>
        public RegistryBuilder<T> Registry { get; }

        public RegistryBuildingEvent(RegistryBuilder<T> registry)
        {
            Registry = registry;
        }
    }
}