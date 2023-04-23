using NUnit.Framework;
using SharpDX;
using FluentAssertions;
using CipherPark.Aurora.Core.World.Collision;

namespace Aurora.Core.Tests.Integration
{
    public class CollisionDetectorIntegrationTests
    {
        [Test]
        public void When_ObserveCollidingSpheres_CollisionEventsCreated()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var collider1 = CollisionTestHelper.CreateSphereCollider(origin + size / 4f, size);
            var collider2 = CollisionTestHelper.CreateSphereCollider(origin, size);
            var sut = new CollisionDetector();

            //act
            var collisionEvent = collider1.DetectCollision(null, collider2, null);

            //assert
            collisionEvent.Should().NotBeNull();
            collisionEvent.Collider1.Should().Be(collider1);
            collisionEvent.Collider2.Should().Be(collider2);
        }
        
        [Test]
        public void When_SphereDisjointFromSphere_CollisionNotDetected()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CollisionTestHelper.CreateSphereCollider(origin + size * 2, size);
            var targetCollider = CollisionTestHelper.CreateSphereCollider(origin, size);

            //act
            var collisionEvent = sut.DetectCollision(null, targetCollider, null);

            //assert
            collisionEvent.Should().BeNull();
        }

        [Test]
        public void When_SphereIntersectsBox_CollisionDetected()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var targetCollider = CollisionTestHelper.CreateBoxCollider(origin, size);
            var sut = CollisionTestHelper.CreateSphereCollider(origin + new Vector3(size, 0, 0), radius);

            //act
            var collisionEvent = sut.DetectCollision(null, targetCollider, null);

            //assert
            collisionEvent.Should().NotBeNull();
            collisionEvent.Collider1.Should().Be(sut);
            collisionEvent.Collider2.Should().Be(targetCollider);
        }
    }
}