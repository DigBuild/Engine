using System;

namespace DigBuild.Engine.Items
{
    public interface IInventorySlot
    {
        public ItemInstance Item { get; set; }
    }

    public sealed class InventorySlot : IInventorySlot
    {
        private ItemInstance _item;

        public ItemInstance Item
        {
            get => _item;
            set
            {
                _item = value;
                Changed?.Invoke();
            }
        }

        public event Action? Changed;
        
        public InventorySlot(ItemInstance item)
        {
            _item = item;
        }

        public InventorySlot() : this(ItemInstance.Empty)
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