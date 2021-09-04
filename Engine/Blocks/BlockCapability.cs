using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block capability.
    /// </summary>
    public interface IBlockCapability
    {
        internal Func<IBlockContext, object> GenericDefaultValueDelegate { get; }
    }
    
    /// <summary>
    /// A block capability.
    /// </summary>
    /// <typeparam name="T">The capability type</typeparam>
    public sealed class BlockCapability<T> : IBlockCapability
    {
        private readonly Func<IBlockContext, object> _default;

        internal BlockCapability(Func<IBlockContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IBlockContext, object> IBlockCapability.GenericDefaultValueDelegate => _default;
    }

    /// <summary>
    /// Registry extensions for block capabilities.
    /// </summary>
    public static class BlockCapabilityRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static BlockCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IBlockCapability> builder,
            ResourceName name,
            Func<IBlockContext, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new BlockCapability<TCap>(defaultValueDelegate));
        }
        
        /// <summary>
        /// Registers a new capability.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The capability</returns>
        public static BlockCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IBlockCapability> builder,
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
        public static BlockCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IBlockCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}