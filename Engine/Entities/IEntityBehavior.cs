namespace DigBuild.Engine.Entities
{
    public interface IEntityBehavior<TContract>
    {
        void Init(TContract data) { }
        void Build(EntityBehaviorBuilder<TContract> entity);
    }
}