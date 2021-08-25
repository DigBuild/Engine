using System;

namespace DigBuild.Engine.Items.Inventories
{
    public interface IInventoryTransaction
    {
        ItemInstance Insert(ItemInstance item);
        ItemInstance Extract(ushort amount, Func<IReadOnlyItemInstance, bool> test = null!);

        void Commit();
    }

    public interface IAdvancedInventoryTransaction : IInventoryTransaction
    {
        void Checkpoint();
        void Rollback();
    }
}