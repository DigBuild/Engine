﻿using System;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public interface IEntityAttribute
    {
        internal Func<IReadOnlyEntityInstance, object> GenericDefaultValueDelegate { get; }
    }

    public sealed class EntityAttribute<T> : IEntityAttribute
    {
        private readonly Func<IReadOnlyEntityInstance, object> _default;

        internal EntityAttribute(Func<IReadOnlyEntityInstance, T> defaultValueDelegate)
        {
            _default = ctx => defaultValueDelegate(ctx)!;
        }

        Func<IReadOnlyEntityInstance, object> IEntityAttribute.GenericDefaultValueDelegate => _default;
    }

    public static class EntityAttributeRegistryBuilderExtensions
    {
        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            Func<IReadOnlyEntityInstance, TAttrib> defaultValueDelegate
        )
        {
            return builder.Add(name, new EntityAttribute<TAttrib>(defaultValueDelegate));
        }

        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            Func<TAttrib> defaultValueDelegate
        ) => Register(builder, name, _ => defaultValueDelegate());

        public static EntityAttribute<TAttrib> Register<TAttrib>(
            this IRegistryBuilder<IEntityAttribute> builder,
            ResourceName name,
            TAttrib defaultValue
        ) => Register(builder, name, _ => defaultValue);
    }
}