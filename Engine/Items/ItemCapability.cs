using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public interface IItemCapability
    {
        internal Func<ItemInstance, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class ItemCapability<T> : IItemCapability
    {
        private readonly Func<ItemInstance, object> _default;

        internal ItemCapability(Func<ItemInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<ItemInstance, object> IItemCapability.GenericDefaultValueDelegate => _default;
    }

    public static class ItemCapabilityRegistryBuilderExtensions
    {
        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            Func<ItemInstance, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new ItemCapability<TCap>(defaultValueDelegate));
        }

        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            Func<TCap> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());

        public static ItemCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IItemCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}