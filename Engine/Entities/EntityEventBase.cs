namespace DigBuild.Engine.Entities
{
    public abstract class EntityEventBase : IEntityEvent
    {
        public EntityInstance Entity { get; }

        protected EntityEventBase(EntityInstance entity)
        {
            Entity = entity;
        }
    }
}