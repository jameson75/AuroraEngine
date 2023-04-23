using CipherPark.Aurora.Core.World.Collision;
using SharpDX;

namespace Aurora.Core.Tests.Integration
{
    public static class CollisionTestHelper
    {
        public static BoxCollider CreateBoxCollider(Vector3 origin, float size)
        {
            return new BoxCollider()
            {
                Box = Unit.BoundingBoxOAUnitTests.CreateBox(origin, size),
            };
        }

        public static SphereCollider CreateSphereCollider(Vector3 origin, float radius)
        {
            return new SphereCollider()
            {
                Sphere = new BoundingSphere(origin, radius),
            };
        }
    }
}
