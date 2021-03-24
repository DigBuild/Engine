namespace DigBuild.Engine.Items
{
    public interface IReadOnlyInventorySlot
    {
        public IReadOnlyItemInstance Item { get; }
    }
}