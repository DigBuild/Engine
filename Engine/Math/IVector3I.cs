using System;
using System.Runtime.CompilerServices;

namespace DigBuild.Engine.Math
{
    public interface IVector3I : IEquatable<IVector3I>
    {
        int X { get; }
        int Y { get; }
        int Z { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double Length()
        {
            return System.Math.Sqrt(LengthSquared());
        }
    }
}