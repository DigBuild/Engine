using System;
using DigBuild.Engine.Serialization;

namespace DigBuild.Engine.Items.Inventories
{
    public sealed class InventorySlot : IInventorySlot
    {
        private static readonly Func<ItemInstance, bool> Always = _ => true;

        private readonly Func<ItemInstance, bool> _test;

        public ItemInstance Item { get; private set; }

        public event Action? Changed;
        
        public InventorySlot(Func<ItemInstance, bool> test, ItemInstance item)
        {
            _test = test;
            Item = item;
        }

        public InventorySlot(Func<ItemInstance, bool> test) : this(test, ItemInstance.Empty)
        {
        }
        
        public InventorySlot(ItemInstance item) : this(Always, item)
        {
        }

        public InventorySlot() : this(ItemInstance.Empty)
        {
        }

        public bool TrySetItem(ItemInstance value, bool doSet = true)
        {
            if (!_test(value))
                return false;

            if (doSet)
            {
                Item = value;
                Changed?.Invoke();
            }

            return true;
        }

        public override string ToString()
        {
            return Item.ToString();
        }

        public static ISerdes<InventorySlot> Serdes { get; } = new CompositeSerdes<InventorySlot>()
        {
            { 1u, slot => slot.Item, ItemInstance.Serdes }
        };
    }
}