namespace DigBuild.Engine.Items.Inventories
{
    /// <summary>
    /// A read-only lockable inventory slot.
    /// </summary>
    public interface IReadOnlyLockableInventorySlot : IReadOnlyInventorySlot
    {
        /// <summary>
        /// Whether it is locked or not.
        /// </summary>
        bool IsLocked { get; }
    }

    /// <summary>
    /// A lockable inventory slot.
    /// </summary>
    public interface ILockableInventorySlot : IInventorySlot, IReadOnlyLockableInventorySlot
    {
        /// <summary>
        /// Toggle the locked state.
        /// </summary>
        void ToggleLocked();
    }
}