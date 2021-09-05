using System.IO;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    /// <summary>
    /// An opaque handle.
    /// </summary>
    public interface IDataHandle
    {
        internal ResourceName? Name { get; }

        internal void Serialize(Stream stream, IData obj);
        internal IData Deserialize(Stream stream, IDeserializationContext context);
    }
    
    /// <summary>
    /// An opaque handle for a target.
    /// </summary>
    /// <typeparam name="TTarget">The target</typeparam>
    public interface IDataHandle<TTarget> : IDataHandle
    {
    }
}