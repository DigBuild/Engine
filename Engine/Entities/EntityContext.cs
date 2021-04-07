namespace DigBuild.Engine.Entities
{
    public class EntityContext : IEntityContext
    {
        public EntityInstance Entity { get; }

        public EntityContext(EntityInstance entity)
        {
            Entity = entity;
        }
    }
}