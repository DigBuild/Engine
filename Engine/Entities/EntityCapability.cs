using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An entity capability.
    /// </summary>
    public interface IEntityCapability
    {
        internal Func<EntityInstance, object> GenericDefaultValueDelegate { get; }
    }
    
    /// <summary>
    /// An entity capability.
    /// </summary>
    /// <typeparam name="T">The capability type</typeparam>
    public sealed class EntityCapability<T> : IEntityCapability
    {
        private readonly Func<EntityInstance, object> _default;

        internal EntityCapability(Func<EntityInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<EntityInstance, object> IEntityCapability.GenericDefaultValueDelegate => _default;
    }
    
    /// <summary>
    /// Registry extensions for entity capabilities.
    /// </summary>
    public static class EntityCapabilityRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            Func<EntityInstance, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new EntityCapability<TCap>(defaultValueDelegate));
        }
        
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The capability</returns>
        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            Func<TCap> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());
        
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The capability</returns>
        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}