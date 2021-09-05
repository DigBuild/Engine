namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An internal item event.
    /// </summary>
    public abstract class ItemEventBase : IItemEvent
    {
        /// <summary>
        /// The item.
        /// </summary>
        public ItemInstance Item { get; }

        protected ItemEventBase(ItemInstance item)
        {
            Item = item;
        }
    }
}