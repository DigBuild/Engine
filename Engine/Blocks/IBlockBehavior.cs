namespace DigBuild.Engine.Blocks
{
    public interface IBlockBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        void Init(TContract data) { }
        void Build(BlockBehaviorBuilder<TReadOnlyContract, TContract> block);
    }

    public interface IBlockBehavior<TContract> : IBlockBehavior<TContract, TContract>
    {
    }

    public interface IBlockBehavior : IBlockBehavior<object>
    {
    }
}