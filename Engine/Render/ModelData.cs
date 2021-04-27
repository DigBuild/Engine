using System;
using System.Collections.Generic;
using DigBuild.Engine.Blocks;

namespace DigBuild.Engine.Render
{
    public sealed class ModelData : IReadOnlyModelData
    {
        public static BlockAttribute<ModelData> Attribute { get; internal set; } = null!;

        private readonly Dictionary<Type, object> _data = new();

        public ModelData CreateOrExtend<T>(Action<T> action) where T : notnull, new()
        {
            if (!_data.TryGetValue(typeof(T), out var value))
                _data[typeof(T)] = value = new T();
            action((T) value);
            return this;
        }

        public T? Get<T>() where T : notnull
        {
            return (T?) _data.GetValueOrDefault(typeof(T));
        }
    }
}