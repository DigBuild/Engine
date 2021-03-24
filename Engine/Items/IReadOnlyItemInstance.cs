using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items
{
    public interface IReadOnlyItemInstance
    {
        public Item Type { get; }
        public ushort Count { get; }
        internal DataContainer DataContainer { get; }
    }
}