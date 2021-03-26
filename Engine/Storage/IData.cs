namespace DigBuild.Engine.Storage
{
    public interface IData
    {
        internal IData Copy();
    }

    public interface IData<out T> : IData where T : IData<T>
    {
        new T Copy();
        IData IData.Copy() => Copy();
    }
}