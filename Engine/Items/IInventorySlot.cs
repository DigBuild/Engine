namespace DigBuild.Engine.Items
{
    public interface IInventorySlot : IReadOnlyInventorySlot
    {
        public new ItemInstance Item { get; set; }
        IReadOnlyItemInstance IReadOnlyInventorySlot.Item => Item;
    }
}