using System.Diagnostics.CodeAnalysis;

namespace DigBuild.Engine.Physics
{
    public interface IRayCollider<THit>
    {
        bool TryCollide(Raycast.Ray ray, [NotNullWhen(true)] out THit? hit);
    }
}