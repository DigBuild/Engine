namespace DigBuild.Engine.Items
{
    public sealed class ItemInstance
    {
        public static readonly ItemInstance Empty = new(null!, 0);

        public Item Type { get; }
        public ushort Count { get; set; }
        internal ItemDataContainer DataContainer { get; } = new();

        public ItemInstance(Item type, ushort count)
        {
            Type = type;
            Count = count;
        }

        public override string ToString()
        {
            return Count == 0 ? "Empty" : $"{Count}x {Type.Name}";
        }
    }
}