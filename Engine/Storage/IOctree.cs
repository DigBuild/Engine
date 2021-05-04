namespace DigBuild.Engine.Storage
{
    public interface IOctree<T> : IReadOnlyOctree<T>
    {
        new T this[int x, int y, int z] { get; set; }
    }
}