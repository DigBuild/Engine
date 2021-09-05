namespace DigBuild.Engine.Items.Inventories
{
    /// <summary>
    /// A read-only inventory slot.
    /// </summary>
    public interface IReadOnlyInventorySlot
    {
        /// <summary>
        /// The item in the slot.
        /// </summary>
        IReadOnlyItemInstance Item { get; }
    }
}