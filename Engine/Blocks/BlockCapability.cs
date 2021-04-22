using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public interface IBlockCapability
    {
        internal Func<IBlockContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class BlockCapability<T> : IBlockCapability
    {
        private readonly Func<IBlockContext, object> _default;

        internal BlockCapability(Func<IBlockContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IBlockContext, object> IBlockCapability.GenericDefaultValueDelegate => _default;
    }

    public static class BlockCapabilityRegistryBuilderExtensions
    {
        public static BlockCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IBlockCapability> builder,
            ResourceName name,
            Func<IBlockContext, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new BlockCapability<TCap>(defaultValueDelegate));
        }

        public static BlockCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IBlockCapability> builder,
            ResourceName name,
            Func<TCap> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());

        public static BlockCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IBlockCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}