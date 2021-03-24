namespace DigBuild.Engine.Entities
{
    public interface IEntityContext : IReadOnlyEntityContext
    {
        public new EntityInstance Entity { get; }
        IReadOnlyEntityInstance IReadOnlyEntityContext.Entity => Entity;
    }
}