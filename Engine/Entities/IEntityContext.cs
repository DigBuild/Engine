namespace DigBuild.Engine.Entities
{
    public interface IEntityContext
    {
        public EntityInstance Entity { get; }
    }

    public sealed class EntityContext : IEntityContext
    {
        public EntityInstance Entity { get; }

        public EntityContext(EntityInstance entity)
        {
            Entity = entity;
        }
    }
}