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
    /// <summary>
    /// A read-only view of the entities in a world.
    /// </summary>
    public interface IReadOnlyWorldEntities : IEnumerable<EntityInstance>
    {
        /// <summary>
        /// Gets an entity by ID.
        /// </summary>
        /// <param name="guid">The ID</param>
        /// <returns>The entity, or null if missing</returns>
        EntityInstance? Get(Guid guid);

        /// <summary>
        /// Tries to get an entity by ID.
        /// </summary>
        /// <param name="guid">The ID</param>
        /// <param name="entity">The entity</param>
        /// <returns>Whether the entity was found or not</returns>
        bool TryGet(Guid guid, [MaybeNullWhen(false)] out EntityInstance entity);

        /// <summary>
        /// Enumerates all entities of a given type.
        /// </summary>
        /// <param name="entity">The entity type</param>
        /// <returns>The enumeration</returns>
        IEnumerable<EntityInstance> OfType(Entity entity);
    }
    
    /// <summary>
    /// A world data type for entity storage.
    /// </summary>
    public class WorldEntities : IReadOnlyWorldEntities, IData<WorldEntities>, IChangeNotifier
    {
        /// <summary>
        /// The data handle.
        /// </summary>
        public static DataHandle<IWorld, IReadOnlyWorldEntities, WorldEntities> Type { get; internal set; } = null!;

        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public event Action? Changed;

        /// <summary>
        /// Begins tracking an entity.
        /// </summary>
        /// <param name="entity">The entity</param>
        public void Track(EntityInstance entity)
        {
            _entities.TryAdd(entity.Id, entity);
        }

        /// <summary>
        /// Stops tracking an entity.
        /// </summary>
        /// <param name="guid">The entity ID</param>
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

        internal static void OnChunkLoaded(BuiltInChunkEvent.Loaded evt)
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

        internal static void OnChunkUnloaded(BuiltInChunkEvent.Unloading evt)
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

        /// <summary>
        /// The serdes.
        /// </summary>
        public static ISerdes<WorldEntities> Serdes { get; } = EmptySerdes<WorldEntities>.Instance;
    }

    /// <summary>
    /// Helpers for accessing entities.
    /// </summary>
    public static class WorldEntitiesExtensions
    {
        /// <summary>
        /// Enumerates all the entities in the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The entities</returns>
        public static IEnumerable<EntityInstance> GetEntities(this IReadOnlyWorld world)
        {
            return world.Get(WorldEntities.Type);
        }
        /// <summary>
        /// Enumerates all the entities of a given type in the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="type">The entity type</param>
        /// <returns>The entities</returns>
        public static IEnumerable<EntityInstance> GetEntities(this IReadOnlyWorld world, Entity type)
        {
            return world.Get(WorldEntities.Type).OfType(type);
        }

        /// <summary>
        /// Finds an entity by ID.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="guid">The ID</param>
        /// <returns>The entity, or null if missing</returns>
        public static EntityInstance? GetEntity(this IReadOnlyWorld world, Guid guid)
        {
            return world.Get(WorldEntities.Type).Get(guid);
        }
        
        /// <summary>
        /// Adds a new entity of the specified type to the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="type">The entity type</param>
        /// <returns>The entity instance</returns>
        public static EntityInstance AddEntity(this IWorld world, Entity type)
        {
            return AddEntity(world, type, Guid.NewGuid());
        }
        
        /// <summary>
        /// Adds a new entity of the specified type with a specific ID to the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="type">The entity type</param>
        /// <param name="guid">The entity ID</param>
        /// <returns>The entity instance</returns>
        public static EntityInstance AddEntity(this IWorld world, Entity type, Guid guid)
        {
            var entity = new EntityInstance(world, guid, type);
            world.Get(WorldEntities.Type).Track(entity);
            world.OnEntityAdded(entity);
            type.OnJoinedWorld(entity);
            return entity;
        }

        /// <summary>
        /// Removes an entity from the world.
        /// </summary>
        /// <param name="world">The world</param>
        /// <param name="guid">The entity ID</param>
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