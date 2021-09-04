namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A range.
    /// </summary>
    public interface IRangeT { }

    /// <summary>
    /// A range.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public readonly struct RangeT<T> : IRangeT
    {
        /// <summary>
        /// The start of the range.
        /// </summary>
        public T Start { get; }
        /// <summary>
        /// The end of the range.
        /// </summary>
        public T End { get; }

        public RangeT(T start, T end)
        {
            Start = start;
            End = end;
        }
    }
}