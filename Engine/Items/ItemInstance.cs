using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items
{
    public sealed class ItemInstance : IReadOnlyItemInstance
    {
        public static readonly ItemInstance Empty = new(null!, 0);

        public Item Type { get; }
        public ushort Count { get; set; }
        internal DataContainer DataContainer { get; }
        DataContainer IReadOnlyItemInstance.DataContainer => DataContainer;

        public ItemInstance(Item type, ushort count)
        {
            Type = type;
            Count = count;
            DataContainer = type?.CreateDataContainer()!;
        }

        private ItemInstance(Item type, ushort count, DataContainer dataContainer)
        {
            Type = type;
            Count = count;
            DataContainer = dataContainer;
        }

        public override string ToString()
        {
            return Count == 0 ? "Empty" : $"{Count}x {Type.Name}";
        }

        public ItemInstance Copy()
        {
            return new(Type, Count, DataContainer.Copy());
        }
    }
}