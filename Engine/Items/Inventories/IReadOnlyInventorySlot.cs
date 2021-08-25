namespace DigBuild.Engine.Items.Inventories
{
    public interface IReadOnlyInventorySlot
    {
        public IReadOnlyItemInstance Item { get; }
    }
}