using System;

namespace DigBuild.Engine.Items
{
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
}