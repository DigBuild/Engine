using DigBuild.Engine.Events;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.BuiltIn
{
    /// <summary>
    /// A built-in chunk event.
    /// </summary>
    public abstract class BuiltInChunkEvent : IEvent
    {
        private BuiltInChunkEvent()
        {
        }

        /// <summary>
        /// Fired when the chunk has been loaded.
        /// </summary>
        public sealed class Loaded : BuiltInChunkEvent
        {
            /// <summary>
            /// The world.
            /// </summary>
            public IWorld World { get; }
            /// <summary>
            /// The chunk.
            /// </summary>
            public IChunk Chunk { get; }

            public Loaded(IWorld world, IChunk chunk)
            {
                World = world;
                Chunk = chunk;
            }
        }

        /// <summary>
        /// Fired when the chunk is being unloaded.
        /// </summary>
        public sealed class Unloading : BuiltInChunkEvent
        {
            /// <summary>
            /// The world.
            /// </summary>
            public IWorld World { get; }
            /// <summary>
            /// The chunk.
            /// </summary>
            public IChunk Chunk { get; }

            public Unloading(IWorld world, IChunk chunk)
            {
                World = world;
                Chunk = chunk;
            }
        }
    }
}