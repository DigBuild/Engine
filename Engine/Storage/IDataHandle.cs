namespace DigBuild.Engine.Storage
{
    public interface IDataHandle
    {
    }

    public sealed class DataHandle<T> : IDataHandle where T : class, new()
    {
        internal DataHandle()
        {
        }
    }
}