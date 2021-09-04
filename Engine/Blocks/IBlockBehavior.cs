namespace DigBuild.Engine.Blocks
{
    /// <summary>
    /// A block behavior.
    /// </summary>
    /// <typeparam name="TReadOnlyContract">The read-only contract</typeparam>
    /// <typeparam name="TContract">The read-write contract</typeparam>
    public interface IBlockBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        void Init(TContract data) { }
        void Build(BlockBehaviorBuilder<TReadOnlyContract, TContract> block);
    }

    /// <summary>
    /// A block behavior.
    /// </summary>
    /// <typeparam name="TContract">The contract</typeparam>
    public interface IBlockBehavior<TContract> : IBlockBehavior<TContract, TContract>
    {
    }

    /// <summary>
    /// A block behavior.
    /// </summary>
    public interface IBlockBehavior : IBlockBehavior<object>
    {
    }
}