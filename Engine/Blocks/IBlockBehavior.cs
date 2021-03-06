namespace DigBuild.Engine.Blocks
{
    public interface IBlockBehavior<TContract>
    {
        void Init(TContract data) { }
        void Build(BlockBehaviorBuilder<TContract> block);
    }
}