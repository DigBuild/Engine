using System;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Entities
{
    public interface IReadOnlyEntityInstance
    {
        public IReadOnlyWorld World { get; }
        public Guid Id { get; }
        public Entity Type { get; }
        internal DataContainer? DataContainer { get; }

        EntityInstance Copy();
        
        public TAttrib Get<TAttrib>(EntityAttribute<TAttrib> attribute) => Type.Get(this, attribute);
    }
}