using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public interface IItemCapability
    {
        internal Func<IItemContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class ItemCapability<T> : IItemCapability
    {
        private readonly Func<IItemContext, object> _default;

        internal ItemCapability(Func<IItemContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IItemContext, object> IItemCapability.GenericDefaultValueDelegate => _default;
    }

    public static class ItemCapabilityRegistryBuilderExtensions
    {
        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            Func<IItemContext, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new ItemCapability<TCap>(defaultValueDelegate));
        }

        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            Func<TCap> defaultValueDelegate
        ) => Register(builder, name, ctx => defaultValueDelegate());

        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, ctx => defaultValue);
    }
}