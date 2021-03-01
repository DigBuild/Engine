namespace DigBuild.Engine.Voxel
{
    public interface IBlockBehavior<TContract>
    {
        void Build(BlockBehaviorBuilder<TContract> block);
        void Init(TContract data) { }
    }

    public sealed class BlockBehaviorBuilder<TData>
    {

    }
}