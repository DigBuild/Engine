using DigBuild.Engine.Events;

namespace DigBuild.Engine.Worlds
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