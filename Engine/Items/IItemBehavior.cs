namespace DigBuild.Engine.Items
{
    public interface IItemBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        void Init(TContract data) { }
        void Build(ItemBehaviorBuilder<TReadOnlyContract, TContract> item);
        bool Equals(TReadOnlyContract first, TReadOnlyContract second);
    }

    public interface IItemBehavior<TContract> : IItemBehavior<TContract, TContract>
    {
    }

    public interface IItemBehavior : IItemBehavior<object, object>
    {
        bool IItemBehavior<object, object>.Equals(object first, object second) => true;
    }
}