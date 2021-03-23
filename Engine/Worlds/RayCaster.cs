using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Worlds
{
    public static class RayCaster
    {
        public readonly struct Ray
        {
            public readonly Vector3 Start, End;

            public Ray(Vector3 start, Vector3 end)
            {
                Start = start;
                End = end;
            }
        }
        
        public static THit? Cast<THit>(IGridAlignedRayCastingContext<THit> context, Ray ray) where THit : class
        {
            return TryCast(context, ray, out var hit) ? hit : null;
        }

        public static bool TryCast<THit>(IGridAlignedRayCastingContext<THit> context, Ray ray, [NotNullWhen(true)] out THit? hit)
            where THit : class
        {
            var startToEnd = ray.End - ray.Start;
            var direction = Vector3.Normalize(startToEnd);
            var xLen = (double) (direction / direction.X).Length();
            var yLen = (double) (direction / direction.Y).Length();
            var zLen = (double) (direction / direction.Z).Length();
            var reach = (double) startToEnd.Length();
            
            var distanceFromStart = 0d;

            var pos = new Vector3i(
                (int) (direction.X > 0 ? System.Math.Ceiling(ray.Start.X) - 1 : System.Math.Floor(ray.Start.X)),
                (int) (direction.Y > 0 ? System.Math.Ceiling(ray.Start.Y) - 1 : System.Math.Floor(ray.Start.Y)),
                (int) (direction.Z > 0 ? System.Math.Ceiling(ray.Start.Z) - 1 : System.Math.Floor(ray.Start.Z))
            );
            var xOff = direction.X > 0 ? 1 + pos.X - ray.Start.X : ray.Start.X - pos.X;
            var yOff = direction.Y > 0 ? 1 + pos.Y - ray.Start.Y : ray.Start.Y - pos.Y;
            var zOff = direction.Z > 0 ? 1 + pos.Z - ray.Start.Z : ray.Start.Z - pos.Z;

            var xDist = double.IsNaN(xLen) ? double.PositiveInfinity : System.Math.Abs(xOff * xLen);
            var yDist = double.IsNaN(yLen) ? double.PositiveInfinity : System.Math.Abs(yOff * yLen);
            var zDist = double.IsNaN(zLen) ? double.PositiveInfinity : System.Math.Abs(zOff * zLen);
            
            while (distanceFromStart <= reach)
            {
                if (context.Visit(pos, ray.Start + direction * (float) distanceFromStart, ray, out hit))
                    return true;

                if (xDist < yDist)
                {
                    if (xDist < zDist)
                    {
                        distanceFromStart = xDist;
                        xDist += xLen;
                        pos += new Vector3i(direction.X > 0 ? 1 : -1, 0, 0);
                    }
                    else
                    {
                        distanceFromStart = zDist;
                        zDist += zLen;
                        pos += new Vector3i(0, 0, direction.Z > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (yDist < zDist)
                    {
                        distanceFromStart = yDist;
                        yDist += yLen;
                        pos += new Vector3i(0, direction.Y > 0 ? 1 : -1, 0);
                    }
                    else
                    {
                        distanceFromStart = zDist;
                        zDist += zLen;
                        pos += new Vector3i(0, 0, direction.Z > 0 ? 1 : -1);
                    }
                }
            }

            hit = null;
            return false;
        }
    }

    public interface IGridAlignedRayCastingContext<THit> where THit : class
    {
        bool Visit(Vector3i gridPosition, Vector3 position, RayCaster.Ray ray, [NotNullWhen(true)] out THit? hit);
    }
}