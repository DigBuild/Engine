namespace DigBuild.Engine.Items
{
    public interface IItemBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        void Init(TContract data) { }
        void Build(ItemBehaviorBuilder<TReadOnlyContract, TContract> item);
    }

    public interface IItemBehavior<TContract> : IItemBehavior<TContract, TContract>
    {
    }

    public interface IItemBehavior : IItemBehavior<object, object>
    {
    }
}