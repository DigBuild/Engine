using System;

namespace DigBuild.Engine.Items
{
    public interface IInventorySlot : IReadOnlyInventorySlot
    {
        new ItemInstance Item { get; }
        IReadOnlyItemInstance IReadOnlyInventorySlot.Item => Item;

        event Action Changed;

        bool TrySetItem(ItemInstance value, bool doSet = true);
    }
}