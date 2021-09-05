using System;

namespace DigBuild.Engine.Items.Inventories
{
    /// <summary>
    /// An inventory transaction.
    /// </summary>
    public interface IInventoryTransaction
    {
        /// <summary>
        /// Inserts as many of the given item as possible.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The leftover items</returns>
        ItemInstance Insert(ItemInstance item);

        /// <summary>
        /// Extracts up to the given amount of items, optionally matching a filter.
        /// </summary>
        /// <param name="amount">The amount of items</param>
        /// <param name="test">The filter</param>
        /// <returns>The extracted items</returns>
        ItemInstance Extract(ushort amount, Func<IReadOnlyItemInstance, bool> test = null!);

        /// <summary>
        /// Commits the changes.
        /// </summary>
        void Commit();
    }

    /// <summary>
    /// An advanced inventory transaction supporting checkpoints and rollbacks.
    /// </summary>
    public interface IAdvancedInventoryTransaction : IInventoryTransaction
    {
        /// <summary>
        /// Creates a checkpoint of the inventory in its current state.
        /// </summary>
        void Checkpoint();
        /// <summary>
        /// Rolls back to the latest checkpoint.
        /// </summary>
        void Rollback();
    }
}