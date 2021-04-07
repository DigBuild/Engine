using System.IO;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

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

        public ItemInstance Copy()
        {
            return new(Type, Count, DataContainer.Copy());
        }

        public override string ToString()
        {
            return Count == 0 ? "Empty" : $"{Count}x {Type.Name}";
        }

        public static ISerdes<ItemInstance> Serdes { get; } = new SimpleSerdes<ItemInstance>(
            (stream, item) =>
            {
                var bw = new BinaryWriter(stream);

                bw.Write(item.Count);
                if (item.Count == 0)
                    return;
                
                bw.Write(item.Type.Name.ToString());

                Storage.DataContainer.Serdes.Serialize(stream, item.DataContainer);
            },
            stream =>
            {
                var br = new BinaryReader(stream);

                var count = br.ReadUInt16();
                if (count == 0)
                    return Empty;

                var itemName = ResourceName.Parse(br.ReadString())!.Value;
                var item = BuiltInRegistries.Items.GetOrNull(itemName)!;

                var data = Storage.DataContainer.Serdes.Deserialize(stream);

                return new ItemInstance(item, count, data);
            }
        );
    }
}