namespace DigBuild.Engine.Items
{
    /// <summary>
    /// An item behavior.
    /// </summary>
    /// <typeparam name="TReadOnlyContract">The read-only contract</typeparam>
    /// <typeparam name="TContract">The read-write contract</typeparam>
    public interface IItemBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        /// <summary>
        /// Initializes the contract instance.
        /// </summary>
        /// <param name="data">The data instance</param>
        void Init(TContract data) { }
        
        /// <summary>
        /// Adds event handlers and attribute/capability suppliers to the item.
        /// </summary>
        /// <param name="item">The behavior builder</param>
        void Build(ItemBehaviorBuilder<TReadOnlyContract, TContract> item);

        /// <summary>
        /// Tests two instances of a contract for equality.
        /// </summary>
        /// <param name="first">The first</param>
        /// <param name="second">The second</param>
        /// <returns>Whether they are equal or not</returns>
        bool Equals(TReadOnlyContract first, TReadOnlyContract second);
    }
    
    /// <summary>
    /// An item behavior.
    /// </summary>
    /// <typeparam name="TContract">The contract</typeparam>
    public interface IItemBehavior<TContract> : IItemBehavior<TContract, TContract>
    {
    }
    
    /// <summary>
    /// An item behavior.
    /// </summary>
    public interface IItemBehavior : IItemBehavior<object, object>
    {
        bool IItemBehavior<object, object>.Equals(object first, object second) => true;
    }
}