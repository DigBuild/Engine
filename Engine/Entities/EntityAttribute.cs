using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An entity attribute.
    /// </summary>
    public interface IEntityAttribute
    {
        internal Func<IReadOnlyEntityInstance, object> GenericDefaultValueDelegate { get; }
    }
    
    /// <summary>
    /// An entity attribute.
    /// </summary>
    /// <typeparam name="T">The attribute type</typeparam>
    public sealed class EntityAttribute<T> : IEntityAttribute
    {
        private readonly Func<IReadOnlyEntityInstance, object> _default;

        internal EntityAttribute(Func<IReadOnlyEntityInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyEntityInstance, object> IEntityAttribute.GenericDefaultValueDelegate => _default;
    }
    
    /// <summary>
    /// Registry extensions for entity attributes.
    /// </summary>
    public static class EntityAttributeRegistryBuilderExtensions
    {
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            Func<IReadOnlyEntityInstance, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new EntityAttribute<TAttrib>(defaultValueDelegate));
        }
        
        /// <summary>
        /// Registers a new attribute.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="builder">The registry</param>
        /// <param name="name">The name</param>
        /// <param name="defaultValueDelegate">The default value provider</param>
        /// <returns>The attribute</returns>
        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
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
        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}