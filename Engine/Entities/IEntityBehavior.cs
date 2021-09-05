namespace DigBuild.Engine.Entities
{
    /// <summary>
    /// An entity behavior.
    /// </summary>
    /// <typeparam name="TReadOnlyContract">The read-only contract</typeparam>
    /// <typeparam name="TContract">The read-write contract</typeparam>
    public interface IEntityBehavior<TReadOnlyContract, TContract>
        where TContract : TReadOnlyContract
    {
        /// <summary>
        /// Initializes the contract instance.
        /// </summary>
        /// <param name="data">The data instance</param>
        void Init(TContract data) { }
        
        /// <summary>
        /// Adds event handlers and attribute/capability suppliers to the entity.
        /// </summary>
        /// <param name="entity">The behavior builder</param>
        void Build(EntityBehaviorBuilder<TReadOnlyContract, TContract> entity);
    }
    
    /// <summary>
    /// An entity behavior.
    /// </summary>
    /// <typeparam name="TContract">The contract</typeparam>
    public interface IEntityBehavior<TContract> : IEntityBehavior<TContract, TContract>
    {
    }
    
    /// <summary>
    /// An entity behavior.
    /// </summary>
    public interface IEntityBehavior : IEntityBehavior<object, object>
    {
    }
}