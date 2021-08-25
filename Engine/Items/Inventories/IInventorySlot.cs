using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items.Inventories
{
    public interface IInventorySlot : IReadOnlyInventorySlot, IChangeNotifier
    {
        new ItemInstance Item { get; }
        IReadOnlyItemInstance IReadOnlyInventorySlot.Item => Item;

        bool TrySetItem(ItemInstance value, bool doSet = true);
    }
}