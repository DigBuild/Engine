using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// Fired when a type registry is being built so new entries can be added.
    /// </summary>
    /// <typeparam name="T">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public sealed class TypeRegistryBuildingEvent<T, TValue> : IEvent
    {
        /// <summary>
        /// The registry builder.
        /// </summary>
        public TypeRegistryBuilder<T, TValue> Registry { get; }

        public TypeRegistryBuildingEvent(TypeRegistryBuilder<T, TValue> registry)
        {
            Registry = registry;
        }
    }
}