using NUnit.Framework;
using SharpDX;
using FluentAssertions;

namespace Aurora.Core.Tests.Integration
{
    public class BoxColliderIntegerationTests
    {
        [Test]
        public void When_BoxIntersectsBox_CollisionDetected()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = ColliderTestHelper.CreateBoxCollider(origin, size);
            var targetCollider = ColliderTestHelper.CreateBoxCollider(origin, size - 50);

            //act
            var collisionEvent = sut.DetectCollision(null, targetCollider, null);

            //assert
            collisionEvent.Should().NotBeNull();
            collisionEvent.Collider1.Should().Be(sut);
            collisionEvent.Collider2.Should().Be(targetCollider);
        }

        [Test]
        public void When_BoxIsDisjointWithBox_CollisionNotDetected()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = ColliderTestHelper.CreateBoxCollider(origin, size);
            var targetCollider = ColliderTestHelper.CreateBoxCollider(origin + 200, size);

            //act
            var collisionEvent = sut.DetectCollision(null, targetCollider, null);

            //assert
            collisionEvent.Should().BeNull();
        }

        [Test]
        public void When_BoxIntersectsSphere_CollisionDetected()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = ColliderTestHelper.CreateBoxCollider(origin, size);
            var targetCollider = ColliderTestHelper.CreateSphereCollider(origin + new Vector3(size, 0, 0), radius);

            //act
            var collisionEvent = sut.DetectCollision(null, targetCollider, null);

            //assert
            collisionEvent.Should().NotBeNull();
            collisionEvent.Collider1.Should().Be(sut);
            collisionEvent.Collider2.Should().Be(targetCollider);
        }       
    }
}