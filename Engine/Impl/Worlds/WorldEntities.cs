using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Worlds;

namespace DigBuild.Engine.Impl.Worlds
{
    public interface IReadOnlyWorldEntities : IEnumerable<EntityInstance>
    {
        EntityInstance? Get(Guid guid);

        bool TryGet(Guid guid, [MaybeNullWhen(false)] out EntityInstance entity);

        IEnumerable<EntityInstance> OfType(Entity entity);
    }

    public class WorldEntities : IReadOnlyWorldEntities, IData<WorldEntities>, IChangeNotifier
    {
        public static DataHandle<IWorld, IReadOnlyWorldEntities, WorldEntities> Type { get; internal set; } = null!;

        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public event Action? Changed;

        public EntityInstance Add(IWorld world, Entity type) // TODO: NOT THIS
        {
            var guid = Guid.NewGuid();
            var entity = new EntityInstance(world, guid, type);
            _entities.Add(guid, entity);
            return entity;
        }

        public void Remove(Guid guid)
        {
            _entities.Remove(guid);
        }

        public EntityInstance? Get(Guid guid)
        {
            return TryGet(guid, out var instance) ? instance : null;
        }

        public bool TryGet(Guid guid, [MaybeNullWhen(false)] out EntityInstance entity)
        {
            return _entities.TryGetValue(guid, out entity);
        }

        public IEnumerable<EntityInstance> OfType(Entity entity)
        {
            return this.Where(instance => instance.Type == entity);
        }

        public WorldEntities Copy()
        {
            var ews = new WorldEntities();
            foreach (var (guid, instance) in _entities)
                ews._entities.Add(guid, instance);
            return ews;
        }

        public IEnumerator<EntityInstance> GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static ISerdes<WorldEntities> Serdes { get; } = new SimpleSerdes<WorldEntities>(
            (stream, entities) => { },
            stream => new WorldEntities()
        );
    }

    public static class WorldEntitiesExtensions
    {
        public static EntityInstance? GetEntity(this IReadOnlyWorld world, Guid guid)
        {
            return world.Get(WorldEntities.Type).Get(guid);
        }
        
        public static EntityInstance AddEntity(this IWorld world, Entity type)
        {
            var entity = world.Get(WorldEntities.Type).Add(world, type);
            world.OnEntityAdded(entity);
            type.OnJoinedWorld(entity);
            return entity;
        }

        public static void RemoveEntity(this IWorld world, Guid guid)
        {
            var storage = world.Get(WorldEntities.Type);
            var entity = storage.Get(guid);
            entity?.Type.OnLeavingWorld(entity);
            storage.Remove(guid);
            world.OnEntityRemoved(guid);
        }
    }
}