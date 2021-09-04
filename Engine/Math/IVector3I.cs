using System;
using System.Runtime.CompilerServices;

namespace DigBuild.Engine.Math
{
    /// <summary>
    /// A 3D integer vector.
    /// </summary>
    public interface IVector3I : IEquatable<IVector3I>
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        int X { get; }
        /// <summary>
        /// The Y coordinate.
        /// </summary>
        int Y { get; }
        /// <summary>
        /// The Z coordinate.
        /// </summary>
        int Z { get; }
        
        /// <summary>
        /// Calculates the squared length.
        /// </summary>
        /// <returns>The squared length</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }
        
        /// <summary>
        /// Calculates the length.
        /// </summary>
        /// <returns>The length</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double Length()
        {
            return System.Math.Sqrt(LengthSquared());
        }
    }
}