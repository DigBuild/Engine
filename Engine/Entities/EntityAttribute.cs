using System;
using DigBuild.Engine.Reg;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public interface IEntityAttribute
    {
        internal Func<IEntityContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class EntityAttribute<T> : IEntityAttribute
    {
        private readonly Func<IEntityContext, object> _default;

        internal EntityAttribute(Func<IEntityContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IEntityContext, object> IEntityAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class EntityAttributeRegistryBuilderExtensions
    {
        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            Func<IEntityContext, TAttrib> defaultValueDelegate
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