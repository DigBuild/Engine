namespace DigBuild.Engine.Render
{
    public interface IReadOnlyModelData
    {
        T? Get<T>() where T : notnull;
    }
}