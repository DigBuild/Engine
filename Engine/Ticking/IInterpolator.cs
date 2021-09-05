namespace DigBuild.Engine.Ticking
{
    /// <summary>
    /// An interpolator.
    /// </summary>
    public interface IInterpolator
    {
        /// <summary>
        /// A value between 0 and 1 based on the partial interpolation state.
        /// </summary>
        float Value { get; }
    }
}