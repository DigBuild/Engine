using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DigBuild.Engine.Entities;

namespace DigBuild.Engine.Worlds
{
    public interface IReadOnlyEntityWorldStorage : IReadOnlyWorldStorage, IEnumerable<EntityInstance>
    {
        EntityInstance? Get(Guid guid);

        bool TryGet(Guid guid, [MaybeNullWhen(false)] out EntityInstance entity);

        IEnumerable<EntityInstance> OfType(Entity entity);
    }

    public class EntityWorldStorage : IReadOnlyEntityWorldStorage, IWorldStorage<EntityWorldStorage>
    {
        public static WorldStorageType<IReadOnlyEntityWorldStorage, EntityWorldStorage> Type { get; internal set; } = null!;
        
        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public EntityInstance Add(Entity type)
        {
            var guid = Guid.NewGuid();
            var entity = new EntityInstance(null!, guid, type);
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

        public EntityWorldStorage Copy()
        {
            var ews = new EntityWorldStorage();
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

    public static class EntityWorldStorageExtensions
    {
        public static EntityInstance? GetEntity(this IReadOnlyWorld world, Guid guid)
        {
            return world.Get(EntityWorldStorage.Type).Get(guid);
        }
        
        public static EntityInstance AddEntity(this IWorld world, Entity type)
        {
            var entity = world.Get(EntityWorldStorage.Type).Add(type);
            world.OnEntityAdded(entity);
            return entity;
        }

        public static void RemoveEntity(this IWorld world, Guid guid)
        {
            world.Get(EntityWorldStorage.Type).Remove(guid);
            world.OnEntityRemoved(guid);
        }
    }
}