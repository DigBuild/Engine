namespace DigBuild.Engine.Items
{
    public sealed class ItemInstance
    {
        public static readonly ItemInstance Empty = new(null!, 0);

        public Item Item { get; }
        public ushort Count { get; set; }
        internal ItemDataContainer DataContainer { get; } = new();

        public ItemInstance(Item item, ushort count)
        {
            Item = item;
            Count = count;
        }

        public override string ToString()
        {
            return Count == 0 ? "Empty" : $"{Count}x {Item.Name}";
        }
    }
}