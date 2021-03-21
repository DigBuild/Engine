namespace DigBuild.Engine.Blocks
{
    public interface IBlockBehavior<TContract>
    {
        void Init(IBlockContext context, TContract data) { }
        void Build(BlockBehaviorBuilder<TContract> block);
    }
}