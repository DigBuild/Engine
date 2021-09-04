using DigBuild.Engine.Events;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block building event.
    /// </summary>
    public class BlockBuildingEvent : IEvent
    {
        /// <summary>
        /// The name.
        /// </summary>
        public ResourceName Name { get; }

        /// <summary>
        /// The builder.
        /// </summary>
        public BlockBuilder Builder { get; }

        public BlockBuildingEvent(ResourceName name, BlockBuilder builder)
        {
            Name = name;
            Builder = builder;
        }
    }
}