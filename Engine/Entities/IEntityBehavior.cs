namespace DigBuild.Engine.Entities
{
    public interface IEntityBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        void Init(TContract data) { }
        void Build(EntityBehaviorBuilder<TReadOnlyContract, TContract> entity);
    }

    public interface IEntityBehavior<TContract> : IEntityBehavior<TContract, TContract>
    {
    }

    public interface IEntityBehavior : IEntityBehavior<object, object>
    {
    }
}