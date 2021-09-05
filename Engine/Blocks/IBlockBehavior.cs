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
        /// <summary>
        /// Initializes the contract instance.
        /// </summary>
        /// <param name="data">The data instance</param>
        void Init(TContract data) { }

        /// <summary>
        /// Adds event handlers and attribute/capability suppliers to the block.
        /// </summary>
        /// <param name="block">The behavior builder</param>
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