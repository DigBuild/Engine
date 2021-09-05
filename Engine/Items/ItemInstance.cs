using System;
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
        internal DataContainer? DataContainer { get; }

        DataContainer? IReadOnlyItemInstance.DataContainer => DataContainer;

        public ItemInstance(Item type, ushort count) :
            this(type, count, type?.CreateDataContainer())
        {
        }

        private ItemInstance(Item type, ushort count, DataContainer? dataContainer)
        {
            Type = type;
            Count = count;
            DataContainer = dataContainer;
        }

        public TAttrib Get<TAttrib>(ItemAttribute<TAttrib> attribute) => Type.Get(this, attribute);
        public TCap Get<TCap>(ItemCapability<TCap> capability) => Type.Get(this, capability);

        public bool Equals(IReadOnlyItemInstance other, bool ignoreCount = false, bool testEmpty = false)
        {
            if (!testEmpty && (Count == 0 && other.Count == 0)) return true;
            if (!testEmpty && (Count == 0 || other.Count == 0)) return false;
            if (!ignoreCount && Count != other.Count) return false;

            return Type == other.Type && Type.Equals(DataContainer, other.DataContainer);
        }

        bool IEquatable<IReadOnlyItemInstance>.Equals(IReadOnlyItemInstance? other)
        {
            return other != null && Equals(other);
        }

        public ItemInstance Copy()
        {
            return new ItemInstance(Type, Count, DataContainer?.Copy());
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

                item.Type.DataSerdes.Serialize(stream, item.DataContainer);
            },
            (stream, context) =>
            {
                var br = new BinaryReader(stream);

                var count = br.ReadUInt16();
                if (count == 0)
                    return Empty;

                var itemName = ResourceName.Parse(br.ReadString())!.Value;
                var type = DigBuildEngine.Items.GetOr(itemName)!;

                var data = type.DataSerdes.Deserialize(stream, context);

                return new ItemInstance(type, count, data);
            }
        );
    }
}