using DigBuild.Engine.Events;

namespace DigBuild.Engine.Registries
{
    /// <summary>
    /// Fired when a type registry's contents are locked.
    /// </summary>
    /// <typeparam name="T">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public sealed class TypeRegistryBuiltEvent<T, TValue> : IEvent
    {
        /// <summary>
        /// The registry.
        /// </summary>
        public TypeRegistry<T, TValue> Registry { get; }

        public TypeRegistryBuiltEvent(TypeRegistry<T, TValue> registry)
        {
            Registry = registry;
        }
    }
}