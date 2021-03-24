namespace DigBuild.Engine.Items
{
    public sealed class ItemContext : IItemContext
    {
        public ItemInstance Instance { get; }

        public ItemContext(ItemInstance instance)
        {
            Instance = instance;
        }
    }
}