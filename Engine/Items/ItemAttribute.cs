using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An item attribute.
    /// </summary>
    public interface IItemAttribute
    {
        internal Func<IReadOnlyItemInstance, object> GenericDefaultValueDelegate { get; }
    }
    
    /// <summary>
    /// An item attribute.
    /// </summary>
    /// <typeparam name="T">The attribute type</typeparam>
    public sealed class ItemAttribute<T> : IItemAttribute
    {
        private readonly Func<IReadOnlyItemInstance, object> _default;

        internal ItemAttribute(Func<IReadOnlyItemInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyItemInstance, object> IItemAttribute.GenericDefaultValueDelegate => _default;
    }
    
    /// <summary>
    /// Registry extensions for item attributes.
    /// </summary>
    public static class ItemAttributeRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            Func<IReadOnlyItemInstance, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new ItemAttribute<TAttrib>(defaultValueDelegate));
        }
        
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
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
        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}