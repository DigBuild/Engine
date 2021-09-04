using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block attribute.
    /// </summary>
    public interface IBlockAttribute
    {
        internal Func<IReadOnlyBlockContext, object> GenericDefaultValueDelegate { get; }
    }
    
    /// <summary>
    /// A block attribute.
    /// </summary>
    /// <typeparam name="T">The attribute type</typeparam>
    public sealed class BlockAttribute<T> : IBlockAttribute
    {
        private readonly Func<IReadOnlyBlockContext, object> _default;

        internal BlockAttribute(Func<IReadOnlyBlockContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyBlockContext, object> IBlockAttribute.GenericDefaultValueDelegate => _default;
    }

    /// <summary>
    /// Registry extensions for block attributes.
    /// </summary>
    public static class BlockAttributeRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static BlockAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IBlockAttribute> builder,
            ResourceName name,
            Func<IReadOnlyBlockContext, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new BlockAttribute<TAttrib>(defaultValueDelegate));
        }
        
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static BlockAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IBlockAttribute> builder,
            ResourceName name,
            Func<TAttrib> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());
        
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The attribute</returns>
        public static BlockAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IBlockAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}