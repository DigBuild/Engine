using System.Collections.Generic;

namespace DigBuild.Engine.Items.Inventories
{
    /// <summary>
    /// An inventory representation, capable of iterating its contents and executing transactions.
    /// </summary>
    public interface IInventory : IEnumerable<ItemInstance>
    {
        /// <summary>
        /// Begins a transaction with this inventory.
        /// </summary>
        /// <returns>The transaction</returns>
        IInventoryTransaction BeginTransaction();
    }
}