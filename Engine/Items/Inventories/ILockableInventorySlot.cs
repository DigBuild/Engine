namespace DigBuild.Engine.Items.Inventories
{
    public interface IReadOnlyLockableInventorySlot : IReadOnlyInventorySlot
    {
        bool IsLocked { get; }
    }

    public interface ILockableInventorySlot : IInventorySlot, IReadOnlyLockableInventorySlot
    {
        void ToggleLocked();
    }
}