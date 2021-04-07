namespace DigBuild.Engine.Items
{
    public class ItemContext : IItemContext
    {
        public ItemInstance Instance { get; }

        public ItemContext(ItemInstance instance)
        {
            Instance = instance;
        }
    }
}