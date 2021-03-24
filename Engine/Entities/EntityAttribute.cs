using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public interface IEntityAttribute
    {
        internal Func<IReadOnlyEntityContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class EntityAttribute<T> : IEntityAttribute
    {
        private readonly Func<IReadOnlyEntityContext, object> _default;

        internal EntityAttribute(Func<IReadOnlyEntityContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyEntityContext, object> IEntityAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class EntityAttributeRegistryBuilderExtensions
    {
        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            Func<IReadOnlyEntityContext, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new EntityAttribute<TAttrib>(defaultValueDelegate));
        }

        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            Func<TAttrib> defaultValueDelegate
        ) => Register(builder, name, ctx => defaultValueDelegate());

        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, ctx => defaultValue);
    }
}