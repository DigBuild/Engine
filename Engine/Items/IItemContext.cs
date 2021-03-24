namespace DigBuild.Engine.Items
{
    public interface IItemContext : IReadOnlyItemContext
    {
        public new ItemInstance Instance { get; }
        IReadOnlyItemInstance IReadOnlyItemContext.Instance => Instance;
    }
}