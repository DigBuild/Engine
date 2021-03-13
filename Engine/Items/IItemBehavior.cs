namespace DigBuild.Engine.Items
{
    public interface IItemBehavior<TContract>
    {
        void Init(TContract data) { }
        void Build(ItemBehaviorBuilder<TContract> item);
    }
}