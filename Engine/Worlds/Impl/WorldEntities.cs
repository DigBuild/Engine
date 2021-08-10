using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Worlds.Impl
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

        public void Track(EntityInstance entity)
        {
            _entities.TryAdd(entity.Id, entity);
        }

        public void Untrack(Guid guid)
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

        public static void OnChunkLoaded(BuiltInChunkEvent.Loaded evt)
        {
            var worldEntities = evt.World.Get(Type);
            var chunkEntities = evt.Chunk.Get(ChunkEntities.Type);

            foreach (var entity in chunkEntities.Entities)
            {
                entity.Type.OnJoinedWorld(entity);
                entity.World.OnEntityAdded(entity);
                worldEntities.Track(entity);
            }
        }

        public static void OnChunkUnloaded(BuiltInChunkEvent.Unloaded evt)
        {
            var worldEntities = evt.World.Get(Type);
            var chunkEntities = evt.Chunk.Get(ChunkEntities.Type);

            foreach (var entity in chunkEntities.Entities)
            {
                entity.Type.OnLeavingWorld(entity);
                entity.World.OnEntityRemoving(entity);
                worldEntities.Untrack(entity.Id);
            }
        }

        public static ISerdes<WorldEntities> Serdes { get; } = EmptySerdes<WorldEntities>.Instance;
    }

    public static class WorldEntitiesExtensions
    {
        public static IEnumerable<EntityInstance> GetEntities(this IReadOnlyWorld world)
        {
            return world.Get(WorldEntities.Type);
        }
        public static IEnumerable<EntityInstance> GetEntities(this IReadOnlyWorld world, Entity type)
        {
            return world.Get(WorldEntities.Type).OfType(type);
        }

        public static EntityInstance? GetEntity(this IReadOnlyWorld world, Guid guid)
        {
            return world.Get(WorldEntities.Type).Get(guid);
        }
        
        public static EntityInstance AddEntity(this IWorld world, Entity type)
        {
            return AddEntity(world, type, Guid.NewGuid());
        }
        
        public static EntityInstance AddEntity(this IWorld world, Entity type, Guid guid)
        {
            var entity = new EntityInstance(world, guid, type);
            world.Get(WorldEntities.Type).Track(entity);
            world.OnEntityAdded(entity);
            type.OnJoinedWorld(entity);
            return entity;
        }

        public static void RemoveEntity(this IWorld world, Guid guid)
        {
            var storage = world.Get(WorldEntities.Type);
            var entity = storage.Get(guid);
            if (entity == null)
                return;
            entity.Type.OnLeavingWorld(entity);
            world.OnEntityRemoving(entity);
            storage.Untrack(guid);
        }
    }
}