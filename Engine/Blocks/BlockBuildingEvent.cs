using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    public class BlockBuildingEvent : IEvent
    {
        public ResourceName Name { get; }

        public BlockBuilder Builder { get; }

        public BlockBuildingEvent(ResourceName name, BlockBuilder builder)
        {
            Name = name;
            Builder = builder;
        }
    }
}