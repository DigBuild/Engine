using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public interface IEntityCapability
    {
        internal Func<EntityInstance, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class EntityCapability<T> : IEntityCapability
    {
        private readonly Func<EntityInstance, object> _default;

        internal EntityCapability(Func<EntityInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<EntityInstance, object> IEntityCapability.GenericDefaultValueDelegate => _default;
    }

    public static class EntityCapabilityRegistryBuilderExtensions
    {
        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            Func<EntityInstance, TCap> defaultValueDelegate
        )
        {
            return builder.Add(name, new EntityCapability<TCap>(defaultValueDelegate));
        }

        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            Func<TCap> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());

        public static EntityCapability<TCap> Register<TCap>(
            this IRegistryBuilder<IEntityCapability> builder,
            ResourceName name,
            TCap defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}