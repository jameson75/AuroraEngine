using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Systems;

namespace CipherPark.KillScript.Core.World.Geometry
{
    public interface IRigidBody : ITransformable
    {
        BodyMotion BodyMotion { get; set; }
    }
}
