namespace DigBuild.Engine.Serialization
{
    public interface IDeserializationContext
    {
        T? Get<T>();
    }
}