namespace DigBuild.Engine.Render.Models
{
    public interface IReadOnlyModelData
    {
        T? Get<T>() where T : notnull;
    }
}