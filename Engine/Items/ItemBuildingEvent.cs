using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An item building event.
    /// </summary>
    public class ItemBuildingEvent : IEvent
    {
        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }
        
        /// <summary>
        /// The builder.
        /// </summary>
        public ItemBuilder Builder { get; }

        public ItemBuildingEvent(ResourceName name, ItemBuilder builder)
        {
            Name = name;
            Builder = builder;
        }
    }
}