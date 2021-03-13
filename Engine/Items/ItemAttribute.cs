using System;
using DigBuild.Engine.Reg;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public interface IItemAttribute
    {
        internal Func<IItemContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class ItemAttribute<T> : IItemAttribute
    {
        private readonly Func<IItemContext, object> _default;

        internal ItemAttribute(Func<IItemContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IItemContext, object> IItemAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class ItemAttributeRegistryBuilderExtensions
    {
        public static ItemAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IItemAttribute> builder,
            ResourceName name,
            Func<IItemContext, TAttrib> defaultValueDelegate
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