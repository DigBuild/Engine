using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    public class EntityBuildingEvent : IEvent
    {
        public ResourceName Name { get; }

        public EntityBuilder Builder { get; }

        public EntityBuildingEvent(ResourceName name, EntityBuilder builder)
        {
            Name = name;
            Builder = builder;
        }
    }
}