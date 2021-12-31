using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Systems;

namespace CipherPark.Aurora.Core.World.Geometry
{
    public interface IRigidBody : ITransformable
    {
        BodyMotion BodyMotion { get; set; }
    }
}
