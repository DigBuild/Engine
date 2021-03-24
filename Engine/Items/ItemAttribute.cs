using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public interface IItemAttribute
    {
        internal Func<IReadOnlyItemContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class ItemAttribute<T> : IItemAttribute
    {
        private readonly Func<IReadOnlyItemContext, object> _default;

        internal ItemAttribute(Func<IReadOnlyItemContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyItemContext, object> IItemAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class ItemAttributeRegistryBuilderExtensions
    {
        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            Func<IReadOnlyItemContext, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new ItemAttribute<TAttrib>(defaultValueDelegate));
        }

        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            Func<TAttrib> defaultValueDelegate
        ) => Register(builder, name, ctx => defaultValueDelegate());

        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, ctx => defaultValue);
    }
}