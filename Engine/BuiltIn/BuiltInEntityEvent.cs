using DigBuild.Engine.Entities;

namespace DigBuild.Engine.BuiltIn
{
    /// <summary>
    /// A built-in entity event.
    /// </summary>
    public static class BuiltInEntityEvent
    {
        /// <summary>
        /// Fired when the entity joins the world.
        /// </summary>
        public class JoinedWorld : EntityEventBase
        {
            public JoinedWorld(EntityInstance entity) : base(entity)
            {
            }
        }

        /// <summary>
        /// Fired when an entity is leaving the world.
        /// </summary>
        public class LeavingWorld : EntityEventBase
        {
            public LeavingWorld(EntityInstance entity) : base(entity)
            {
            }
        }
    }
    
    /// <summary>
    /// Registration/subscription extensions for built-in entity events.
    /// </summary>
    public static class BuiltInEntityEventExtensions
    {
        /// <summary>
        /// Subscribes to the joined world event.
        /// </summary>
        /// <typeparam name="TReadOnlyData">The read-only data type</typeparam>
        /// <typeparam name="TData">The read-write data type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="onJoinedWorld">The handler</param>
        public static void Subscribe<TReadOnlyData, TData>(
            this IEntityBehaviorBuilder<TReadOnlyData, TData> builder,
            EntityEventDelegate<TData, BuiltInEntityEvent.JoinedWorld> onJoinedWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onJoinedWorld);
        }
        
        /// <summary>
        /// Fires the joined world event.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="instance">The instance</param>
        public static void OnJoinedWorld(this Entity entity, EntityInstance instance)
        {
            entity.Post(new BuiltInEntityEvent.JoinedWorld(instance));
        }
        
        /// <summary>
        /// Subscribes to the leaving world event.
        /// </summary>
        /// <typeparam name="TReadOnlyData">The read-only data type</typeparam>
        /// <typeparam name="TData">The read-write data type</typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="onLeavingWorld">The handler</param>
        public static void Subscribe<TReadOnlyData, TData>(
            this IEntityBehaviorBuilder<TReadOnlyData, TData> builder,
            EntityEventDelegate<TData, BuiltInEntityEvent.LeavingWorld> onLeavingWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onLeavingWorld);
        }
        
        /// <summary>
        /// Fires the leaving world event.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="instance">The instance</param>
        public static void OnLeavingWorld(this Entity entity, EntityInstance instance)
        {
            entity.Post(new BuiltInEntityEvent.LeavingWorld(instance));
        }
    }
}