using System;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// A read-only instance of an entity.
    /// </summary>
    public interface IReadOnlyEntityInstance
    {
        /// <summary>
        /// The world.
        /// </summary>
        public IReadOnlyWorld World { get; }
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// The entity type.
        /// </summary>
        public Entity Type { get; }
        internal DataContainer? DataContainer { get; }

        /// <summary>
        /// Creates a deep copy of the entity.
        /// </summary>
        /// <returns>The copy</returns>
        EntityInstance Copy();
        
        /// <summary>
        /// Gets an attribute of this entity.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        public TAttrib Get<TAttrib>(EntityAttribute<TAttrib> attribute) => Type.Get(this, attribute);
    }
}