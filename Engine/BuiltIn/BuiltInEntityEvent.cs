using DigBuild.Engine.Entities;

namespace DigBuild.Engine.BuiltIn
{
    public static class BuiltInEntityEvent
    {
        public class JoinedWorld : IEntityEvent<IEntityContext>
        {
        }

        public class LeavingWorld : IEntityEvent<IEntityContext>
        {
        }
    }

    public static class BuiltInEntityEventExtensions
    {
        public static void Subscribe<TReadOnlyData, TData>(
            this IEntityBehaviorBuilder<TReadOnlyData, TData> builder,
            EntityEventDelegate<IEntityContext, TData, BuiltInEntityEvent.JoinedWorld> onJoinedWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onJoinedWorld);
        }

        public static void OnJoinedWorld(this IEntity entity, IEntityContext context, BuiltInEntityEvent.JoinedWorld evt)
        {
            entity.Post(context, evt);
        }

        public static void Subscribe<TReadOnlyData, TData>(
            this IEntityBehaviorBuilder<TReadOnlyData, TData> builder,
            EntityEventDelegate<IEntityContext, TData, BuiltInEntityEvent.LeavingWorld> onLeavingWorld
        )
            where TData : TReadOnlyData
        {
            builder.Subscribe(onLeavingWorld);
        }

        public static void OnLeavingWorld(this IEntity entity, IEntityContext context, BuiltInEntityEvent.LeavingWorld evt)
        {
            entity.Post(context, evt);
        }
    }
}