namespace DigBuild.Engine.Math
{
    public interface IRangeT { }

    public readonly struct RangeT<T> : IRangeT
    {
        public T Start { get; }
        public T End { get; }

        public RangeT(T start, T end)
        {
            Start = start;
            End = end;
        }
    }
}