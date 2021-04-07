using DigBuild.Engine.Entities;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInEntityEvent
    {
        public class JoinedWorld : EntityContext, IEntityEvent
        {
            public JoinedWorld(EntityInstance entity) : base(entity)
            {
            }
        }

        public class LeavingWorld : EntityContext, IEntityEvent
        {
            public LeavingWorld(EntityInstance entity) : base(entity)
            {
            }
        }
    }

    public static class BuiltInEntityEventExtensions
    {
        public static void Subscribe<TReadOnlyData, TData>(
            this IEntityBehaviorBuilder<TReadOnlyData, TData> builder,
            EntityEventDelegate<TData, BuiltInEntityEvent.JoinedWorld> onJoinedWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onJoinedWorld);
        }

        public static void OnJoinedWorld(this Entity entity, EntityInstance instance)
        {
            entity.Post(new BuiltInEntityEvent.JoinedWorld(instance));
        }

        public static void Subscribe<TReadOnlyData, TData>(
            this IEntityBehaviorBuilder<TReadOnlyData, TData> builder,
            EntityEventDelegate<TData, BuiltInEntityEvent.LeavingWorld> onLeavingWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onLeavingWorld);
        }

        public static void OnLeavingWorld(this Entity entity, EntityInstance instance)
        {
            entity.Post(new BuiltInEntityEvent.LeavingWorld(instance));
        }
    }
}