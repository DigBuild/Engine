using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public interface IEntityCapability
    {
        internal Func<IEntityContext, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class EntityCapability<T> : IEntityCapability
    {
        private readonly Func<IEntityContext, object> _default;

        internal EntityCapability(Func<IEntityContext, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IEntityContext, object> IEntityCapability.GenericDefaultValueDelegate => _default;
    }

    public static class EntityCapabilityRegistryBuilderExtensions
    {
        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            Func<IEntityContext, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new EntityCapability<TCap>(defaultValueDelegate));
        }

        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            Func<TCap> defaultValueDelegate
        ) => Register(builder, name, ctx => defaultValueDelegate());

        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, ctx => defaultValue);
    }
}