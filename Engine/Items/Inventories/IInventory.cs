using System.Collections.Generic;

namespace DigBuild.Engine.Items.Inventories
{
    public interface IInventory : IEnumerable<ItemInstance>
    {
        IInventoryTransaction BeginTransaction();
    }
}