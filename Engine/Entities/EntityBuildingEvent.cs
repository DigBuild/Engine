using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An entity building event.
    /// </summary>
    public class EntityBuildingEvent : IEvent
    {
        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }
        
        /// <summary>
        /// The builder.
        /// </summary>
        public EntityBuilder Builder { get; }

        public EntityBuildingEvent(ResourceName name, EntityBuilder builder)
        {
            Name = name;
            Builder = builder;
        }
    }
}