using System;
using DigBuild.Engine.Voxel;

namespace DigBuild.Engine.Entities
{
    public sealed class EntityInstance
    {
        public IWorld World { get; }
        public Guid Id { get; set; }
        public Entity Type { get; }
        internal EntityDataContainer DataContainer { get; }

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