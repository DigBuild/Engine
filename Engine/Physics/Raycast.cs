using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Physics
{
    public static class Raycast
    {
        public readonly struct Ray
        {
            public readonly Vector3 Origin, Magnitude;

            public Ray(Vector3 origin, Vector3 magnitude)
            {
                Origin = origin;
                Magnitude = magnitude;
            }
            
            public static Ray operator +(Ray ray, Vector3 vec) => new(ray.Origin + vec, ray.Magnitude);
            public static Ray operator -(Ray ray, Vector3 vec) => new(ray.Origin - vec, ray.Magnitude);
        }
        
        public static THit? Cast<THit>(IGridAlignedRayCastingContext<THit> context, Ray ray) where THit : class
        {
            return TryCast(context, ray, out var hit) ? hit : null;
        }

        public static bool TryCast<THit>(IGridAlignedRayCastingContext<THit> context, Ray ray, [NotNullWhen(true)] out THit? hit)
            where THit : class
        {
            var direction = Vector3.Normalize(ray.Magnitude);
            var xLen = (double) (direction / direction.X).Length();
            var yLen = (double) (direction / direction.Y).Length();
            var zLen = (double) (direction / direction.Z).Length();
            var reach = (double) ray.Magnitude.Length();
            
            var distanceFromStart = 0d;

            var pos = new Vector3I(
                (int) (direction.X > 0 ? System.Math.Ceiling(ray.Origin.X) - 1 : System.Math.Floor(ray.Origin.X)),
                (int) (direction.Y > 0 ? System.Math.Ceiling(ray.Origin.Y) - 1 : System.Math.Floor(ray.Origin.Y)),
                (int) (direction.Z > 0 ? System.Math.Ceiling(ray.Origin.Z) - 1 : System.Math.Floor(ray.Origin.Z))
            );
            var xOff = direction.X > 0 ? 1 + pos.X - ray.Origin.X : ray.Origin.X - pos.X;
            var yOff = direction.Y > 0 ? 1 + pos.Y - ray.Origin.Y : ray.Origin.Y - pos.Y;
            var zOff = direction.Z > 0 ? 1 + pos.Z - ray.Origin.Z : ray.Origin.Z - pos.Z;

            var xDist = double.IsNaN(xLen) ? double.PositiveInfinity : System.Math.Abs(xOff * xLen);
            var yDist = double.IsNaN(yLen) ? double.PositiveInfinity : System.Math.Abs(yOff * yLen);
            var zDist = double.IsNaN(zLen) ? double.PositiveInfinity : System.Math.Abs(zOff * zLen);
            
            while (distanceFromStart <= reach)
            {
                if (context.Visit(pos, ray.Origin + direction * (float) distanceFromStart, ray, out hit))
                    return true;

                if (xDist < yDist)
                {
                    if (xDist < zDist)
                    {
                        distanceFromStart = xDist;
                        xDist += xLen;
                        pos += new Vector3I(direction.X > 0 ? 1 : -1, 0, 0);
                    }
                    else
                    {
                        distanceFromStart = zDist;
                        zDist += zLen;
                        pos += new Vector3I(0, 0, direction.Z > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (yDist < zDist)
                    {
                        distanceFromStart = yDist;
                        yDist += yLen;
                        pos += new Vector3I(0, direction.Y > 0 ? 1 : -1, 0);
                    }
                    else
                    {
                        distanceFromStart = zDist;
                        zDist += zLen;
                        pos += new Vector3I(0, 0, direction.Z > 0 ? 1 : -1);
                    }
                }
            }

            hit = null;
            return false;
        }
    }
}