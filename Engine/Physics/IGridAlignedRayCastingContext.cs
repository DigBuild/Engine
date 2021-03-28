using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public interface IGridAlignedRayCastingContext<THit> where THit : class
    {
        bool Visit(Vector3I gridPosition, Vector3 position, Raycast.Ray ray, [NotNullWhen(true)] out THit? hit);
    }
}