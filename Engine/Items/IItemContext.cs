namespace DigBuild.Engine.Items
{
    public interface IItemContext
    {
        public ItemInstance Instance { get; }
    }

    public sealed class ItemContext : IItemContext
    {
        public ItemInstance Instance { get; }

        public ItemContext(ItemInstance instance)
        {
            Instance = instance;
        }
    }
}