using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Items
{
    public class ItemBuildingEvent : IEvent
    {
        public ResourceName Name { get; }

        public ItemBuilder Builder { get; }

        public ItemBuildingEvent(ResourceName name, ItemBuilder builder)
        {
            Name = name;
            Builder = builder;
        }
    }
}