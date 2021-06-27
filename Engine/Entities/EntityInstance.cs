using System;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Entities
{
    public sealed class EntityInstance : IReadOnlyEntityInstance
    {
        public IWorld World { get; }
        IReadOnlyWorld IReadOnlyEntityInstance.World => World;
        public Guid Id { get; }
        public Entity Type { get; }
        internal DataContainer DataContainer { get; }
        DataContainer IReadOnlyEntityInstance.DataContainer => DataContainer;

        public EntityInstance(IWorld world, Guid id, Entity type)
        {
            Type = type;
            Id = id;
            World = world;
            DataContainer = type.CreateDataContainer();
        }

        public TAttrib Get<TAttrib>(EntityAttribute<TAttrib> attribute) => Type.Get(this, attribute);
        public TCap Get<TCap>(EntityCapability<TCap> capability) => Type.Get(this, capability);

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