namespace DigBuild.Engine.Worlds
{
    public interface IWorldStorage : IReadOnlyWorldStorage
    {
        public IWorldStorage Copy();
    }

    public interface IWorldStorage<out T> : IWorldStorage where T : IWorldStorage<T>
    {
        public new T Copy();
        IWorldStorage IWorldStorage.Copy() => Copy();
    }
}