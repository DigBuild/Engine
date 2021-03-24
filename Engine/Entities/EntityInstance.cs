using System;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Entities
{
    public sealed class EntityInstance
    {
        public IWorld World { get; }
        public Guid Id { get; set; }
        public Entity Type { get; }
        internal DataContainer DataContainer { get; }

        public EntityInstance(IWorld world, Guid id, Entity type)
        {
            Type = type;
            Id = id;
            World = world;
            DataContainer = type.CreateDataContainer();
        }

        public override string ToString()
        {
            return $"Entity({Type.Name}, {Id})";
        }
    }
}