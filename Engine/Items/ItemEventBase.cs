namespace DigBuild.Engine.Items
{
    public abstract class ItemEventBase : IItemEvent
    {
        public ItemInstance Item { get; }

        protected ItemEventBase(ItemInstance item)
        {
            Item = item;
        }
    }
}