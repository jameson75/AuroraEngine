using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Systems;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public interface IRigidBody : ITransformable
    {
        BodyMotion BodyMotion { get; set; }
    }
}
