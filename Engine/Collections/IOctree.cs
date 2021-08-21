namespace DigBuild.Engine.Collections
{
    public interface IOctree<T> : IReadOnlyOctree<T>
    {
        new T this[int x, int y, int z] { get; set; }
    }
}