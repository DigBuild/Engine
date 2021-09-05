using System;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;
using DigBuild.Engine.Worlds.Impl;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An instance of an entity.
    /// </summary>
    public sealed class EntityInstance : IReadOnlyEntityInstance
    {
        /// <summary>
        /// The world.
        /// </summary>
        public IWorld World { get; }
        IReadOnlyWorld IReadOnlyEntityInstance.World => World;
        public Guid Id { get; }
        public Entity Type { get; }
        internal DataContainer? DataContainer { get; }
        DataContainer? IReadOnlyEntityInstance.DataContainer => DataContainer;

        public EntityInstance(IWorld world, Guid id, Entity type) :
            this(world, id, type, type.CreateDataContainer())
        {
        }

        internal EntityInstance(IWorld world, Guid id, Entity type, DataContainer? dataContainer)
        {
            Type = type;
            Id = id;
            World = world;
            DataContainer = dataContainer;
        }

        public EntityInstance Copy()
        {
            return new EntityInstance(World, Id, Type, DataContainer?.Copy());
        }

        public TAttrib Get<TAttrib>(EntityAttribute<TAttrib> attribute) => Type.Get(this, attribute);
        
        /// <summary>
        /// Gets a capability of this entity.
        /// </summary>
        /// <typeparam name="TCap">The capability type</typeparam>
        /// <param name="capability">The capability</param>
        /// <returns>The value</returns>
        public TCap Get<TCap>(EntityCapability<TCap> capability) => Type.Get(this, capability);

        /// <summary>
        /// Removes this entity from the world.
        /// </summary>
        public void Remove()
        {
            World.RemoveEntity(Id);
        }

        public override string ToString()
        {
            return $"Entity({Type.Name}, {Id})";
        }
    }
}