using DigBuild.Engine.Events;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.BuiltIn
{
    public abstract class BuiltInChunkEvent : IEvent
    {
        private BuiltInChunkEvent()
        {
        }

        public sealed class Loaded : BuiltInChunkEvent
        {
            public IWorld World { get; }
            public IChunk Chunk { get; }

            public Loaded(IWorld world, IChunk chunk)
            {
                World = world;
                Chunk = chunk;
            }
        }

        public sealed class Unloaded : BuiltInChunkEvent
        {
            public IWorld World { get; }
            public IChunk Chunk { get; }

            public Unloaded(IWorld world, IChunk chunk)
            {
                World = world;
                Chunk = chunk;
            }
        }
    }
}