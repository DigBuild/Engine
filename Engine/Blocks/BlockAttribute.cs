using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public interface IBlockAttribute
    {
        internal Func<IReadOnlyBlockContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class BlockAttribute<T> : IBlockAttribute
    {
        private readonly Func<IReadOnlyBlockContext, object> _default;

        internal BlockAttribute(Func<IReadOnlyBlockContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyBlockContext, object> IBlockAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class BlockAttributeRegistryBuilderExtensions
    {
        public static BlockAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IBlockAttribute> builder,
            ResourceName name,
            Func<IReadOnlyBlockContext, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new BlockAttribute<TAttrib>(defaultValueDelegate));
        }

        public static BlockAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IBlockAttribute> builder,
            ResourceName name,
            Func<TAttrib> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());

        public static BlockAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IBlockAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}