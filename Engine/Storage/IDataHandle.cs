using System.IO;
using DigBuild.Engine.Serialization;
using DigBuild.Platform.Resource;

namespace DigBuild.Engine.Storage
{
    public interface IDataHandle
    {
        internal ResourceName? Name { get; }

        internal void Serialize(Stream stream, IData obj);
        internal IData Deserialize(Stream stream, IDeserializationContext context);
    }

    public interface IDataHandle<TTarget> : IDataHandle
    {
    }
}