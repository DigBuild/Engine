using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public interface IItemAttribute
    {
        internal Func<IReadOnlyItemInstance, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class ItemAttribute<T> : IItemAttribute
    {
        private readonly Func<IReadOnlyItemInstance, object> _default;

        internal ItemAttribute(Func<IReadOnlyItemInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyItemInstance, object> IItemAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class ItemAttributeRegistryBuilderExtensions
    {
        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            Func<IReadOnlyItemInstance, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new ItemAttribute<TAttrib>(defaultValueDelegate));
        }

        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            Func<TAttrib> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());

        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}