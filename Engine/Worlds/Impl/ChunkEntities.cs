using System;
using System.Collections.Generic;
using System.IO;
using DigBuild.Engine.BuiltIn;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Serialization;
using DigBuild.Engine.Storage;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Worlds.Impl
{
    /// <summary>
    /// A read-only view of the entities in a chunk.
    /// </summary>
    public interface IReadOnlyChunkEntities : IChangeNotifier
    {
        /// <summary>
        /// The entity enumeration.
        /// </summary>
        IEnumerable<EntityInstance> Entities { get; }
    }

    /// <summary>
    /// A chunk data type for entity storage.
    /// </summary>
    public class ChunkEntities : IReadOnlyChunkEntities, IData<ChunkEntities>
    {
        /// <summary>
        /// The data handle for chunk entities.
        /// </summary>
        public static DataHandle<IChunk, IReadOnlyChunkEntities, ChunkEntities> Type { get; internal set; } = null!;

        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public IEnumerable<EntityInstance> Entities => _entities.Values;

        public event Action? Changed;

        public ChunkEntities()
        {
        }

        /// <summary>
        /// Adds a new entity to the chunk.
        /// </summary>
        /// <param name="entity">The entity</param>
        public void Add(EntityInstance entity)
        {
            _entities.TryAdd(entity.Id, entity);
            NotifyChange();
        }

        /// <summary>
        /// Removes an entity from the chunk.
        /// </summary>
        /// <param name="entity">The entity</param>
        public void Remove(EntityInstance entity)
        {
            _entities.Remove(entity.Id);
            NotifyChange();
        }

        private void NotifyChange()
        {
            Changed?.Invoke();
        }

        public ChunkEntities Copy()
        {
            var copy = new ChunkEntities();
            foreach (var entity in _entities.Values)
                copy._entities.Add(entity.Id, entity.Copy());
            return copy;
        }
        
        // Temporarily disabled due to issues with the save system
        /// <summary>
        /// The serdes.
        /// </summary>
        public static ISerdes<ChunkEntities> Serdes { get; } = EmptySerdes<ChunkEntities>.Instance;

        // public static ISerdes<ChunkEntities> Serdes { get; } = new SimpleSerdes<ChunkEntities>(
        //     (stream, entities) =>
        //     {
        //         var bw = new BinaryWriter(stream);
        //         
        //         bw.Write(entities._entities.Count);
        //
        //         foreach (var entity in entities._entities.Values)
        //         {
        //             bw.Write(entity.Id.ToByteArray());
        //             bw.Write(entity.Type.Name.ToString());
        //
        //             entity.Type.DataSerdes.Serialize(stream, entity.DataContainer);
        //         }
        //     },
        //     (stream, ctx) =>
        //     {
        //         var br = new BinaryReader(stream);
        //
        //         var entities = new ChunkEntities();
        //         var amt = br.ReadInt32();
        //
        //         var world = ctx.Get<IWorld>()!;
        //
        //         for (var i = 0; i < amt; i++)
        //         {
        //             var id = new Guid(br.ReadBytes(16));
        //             
        //             var name = ResourceName.Parse(br.ReadString())!;
        //             var type = BuiltInRegistries.Entities.GetOr(name.Value)!;
        //
        //             var data = type.DataSerdes.Deserialize(stream, ctx);
        //
        //             entities._entities.Add(id, new EntityInstance(world, id, type, data));
        //         }
        //         
        //         return entities;
        //     });
    }
}