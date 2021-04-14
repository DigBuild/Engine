using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
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

    public class World : IReadOnlyWorldEntities, IData<World>, IChangeNotifier
    {
        public static DataHandle<IWorld, IReadOnlyWorldEntities, World> Type { get; internal set; } = null!;
        
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

        public World Copy()
        {
            var ews = new World();
            foreach (var (guid, instance) in _entities)
                ews._entities.Add(guid, instance);
            return ews;
        }

        public IEnumerator<EntityInstance> GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class WorldEntitiesExtensions
    {
        public static EntityInstance? GetEntity(this IReadOnlyWorld world, Guid guid)
        {
            return world.Get(World.Type).Get(guid);
        }
        
        public static EntityInstance AddEntity(this IWorld world, Entity type)
        {
            var entity = world.Get(World.Type).Add(world, type);
            world.OnEntityAdded(entity);
            type.OnJoinedWorld(entity);
            return entity;
        }

        public static void RemoveEntity(this IWorld world, Guid guid)
        {
            var storage = world.Get(World.Type);
            var entity = storage.Get(guid);
            entity?.Type.OnLeavingWorld(entity);
            storage.Remove(guid);
            world.OnEntityRemoved(guid);
        }
    }
}