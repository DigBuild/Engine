using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items.Inventories
{
    /// <summary>
    /// An inventory slot.
    /// </summary>
    public interface IInventorySlot : IReadOnlyInventorySlot, IChangeNotifier
    {
        /// <summary>
        /// The item in the slot.
        /// </summary>
        new ItemInstance Item { get; }
        IReadOnlyItemInstance IReadOnlyInventorySlot.Item => Item;

        /// <summary>
        /// Tries to set the item.
        /// </summary>
        /// <param name="value">The new item</param>
        /// <param name="doSet">Whether to actually set the item or just try</param>
        /// <returns>Whether the operation was successful or not</returns>
        bool TrySetItem(ItemInstance value, bool doSet = true);
    }
}