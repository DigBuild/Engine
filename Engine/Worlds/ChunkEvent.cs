using DigBuild.Engine.Events;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public abstract class ChunkEvent : IEvent
    {
        private ChunkEvent()
        {
        }

        public sealed class Loaded : ChunkEvent
        {
            public IWorld World { get; }
            public IChunk Chunk { get; }

            public Loaded(IWorld world, IChunk chunk)
            {
                World = world;
                Chunk = chunk;
            }
        }

        public sealed class Unloaded : ChunkEvent
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