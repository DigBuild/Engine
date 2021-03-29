using System.Diagnostics.CodeAnalysis;

namespace DigBuild.Engine.Physics
{
    public interface IRayCollider<THit>
    {
        static IRayCollider<THit> None { get; } = new NoCollider();

        bool TryCollide(Raycast.Ray ray, [NotNullWhen(true)] out THit? hit);

        private sealed class NoCollider : IRayCollider<THit>
        {
            public bool TryCollide(Raycast.Ray ray, [NotNullWhen(true)] out THit? hit)
            {
                hit = default;
                return false;
            }
        }
    }
}