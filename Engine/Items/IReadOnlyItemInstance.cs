using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items
{
    public interface IReadOnlyItemInstance
    {
        public Item Type { get; }
        public ushort Count { get; }
        internal DataContainer DataContainer { get; }

        TAttrib Get<TAttrib>(ItemAttribute<TAttrib> attribute);

        bool Equals(IReadOnlyItemInstance other, bool ignoreCount = false, bool testEmpty = false);

        public ItemInstance Copy();
    }
}