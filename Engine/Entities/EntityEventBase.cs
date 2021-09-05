namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An internal entity event.
    /// </summary>
    public abstract class EntityEventBase : IEntityEvent
    {
        /// <summary>
        /// The entity.
        /// </summary>
        public EntityInstance Entity { get; }

        protected EntityEventBase(EntityInstance entity)
        {
            Entity = entity;
        }
    }
}