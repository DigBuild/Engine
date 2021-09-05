using System;
using System.Collections.Generic;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Items;

namespace DigBuild.Engine.Render.Models
{
    /// <summary>
    /// A basic read-write model data implementation.
    /// </summary>
    public sealed class ModelData : IReadOnlyModelData
    {
        /// <summary>
        /// The block attribute.
        /// </summary>
        public static BlockAttribute<ModelData> BlockAttribute { get; internal set; } = null!;
        /// <summary>
        /// The item attribute.
        /// </summary>
        public static ItemAttribute<ModelData> ItemAttribute { get; internal set; } = null!;
        /// <summary>
        /// The entity attribute.
        /// </summary>
        public static EntityAttribute<ModelData> EntityAttribute { get; internal set; } = null!;

        private readonly Dictionary<Type, object> _data = new();

        /// <summary>
        /// Executes the callback on the current instance of the data, or a new one if missing.
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="action">The callback</param>
        /// <returns>The model data</returns>
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