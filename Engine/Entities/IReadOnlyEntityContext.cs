namespace DigBuild.Engine.Entities
{
    public interface IReadOnlyEntityContext
    {
        public IReadOnlyEntityInstance Entity { get; }
    }
}