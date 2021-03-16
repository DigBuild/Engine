using System;

namespace DigBuild.Engine.Items
{
    public interface IInventorySlot
    {
        public ItemInstance Item { get; set; }
    }

    public sealed class InventorySlot : IInventorySlot
    {
        private readonly Action? _notifyChange;
        private ItemInstance _item;

        public ItemInstance Item
        {
            get => _item;
            set
            {
                _item = value;
                _notifyChange?.Invoke();
            }
        }

        public InventorySlot(Action? notifyChange, ItemInstance item)
        {
            _notifyChange = notifyChange;
            _item = item;
        }

        public InventorySlot(Action notifyChange) : this(notifyChange, ItemInstance.Empty)
        {
        }

        public InventorySlot(ItemInstance item) : this(null, item)
        {
        }

        public InventorySlot() : this(null, ItemInstance.Empty)
        {
        }
    }

    public static class InventoryExtensions
    {
        // public static void Insert(this IInventorySlot[] slots, ItemInstance item)
        // {
        // }
    }
}