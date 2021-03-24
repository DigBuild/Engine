using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items
{
    public sealed class ItemInstance : IReadOnlyItemInstance
    {
        public static readonly ItemInstance Empty = new(null!, 0);

        public Item Type { get; }
        public ushort Count { get; set; }
        internal DataContainer DataContainer { get; } = new();
        DataContainer IReadOnlyItemInstance.DataContainer => DataContainer;

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