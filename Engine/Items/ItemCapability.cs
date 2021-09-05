using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An item capability.
    /// </summary>
    public interface IItemCapability
    {
        internal Func<ItemInstance, object> GenericDefaultValueDelegate { get; }
    }
    
    /// <summary>
    /// An item capability.
    /// </summary>
    /// <typeparam name="T">The capability type</typeparam>
    public sealed class ItemCapability<T> : IItemCapability
    {
        private readonly Func<ItemInstance, object> _default;

        internal ItemCapability(Func<ItemInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<ItemInstance, object> IItemCapability.GenericDefaultValueDelegate => _default;
    }
    
    /// <summary>
    /// Registry extensions for item capabilities.
    /// </summary>
    public static class ItemCapabilityRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            Func<ItemInstance, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new ItemCapability<TCap>(defaultValueDelegate));
        }
        
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The capability</returns>
        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
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
        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}