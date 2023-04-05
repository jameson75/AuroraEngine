using NUnit.Framework;
using FluentAssertions;
using CipherPark.Aurora.Core.Utils;
using SharpDX;

namespace Aurora.Core.Tests.Unit
{
    public class BoundingBoxOAUnitTests
    {
        [Test]
        public void When_BoxContainsSmallerBox_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetBox = CreateBox(origin, size - 50);

            //act
            var result = sut.Intersects(targetBox);

            //assert
            result.Should().BeTrue();
        }

        [Test]
        public void When_BoxContainedInLargerBox_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetBox = CreateBox(origin, size + 50);

            //act
            var result = sut.Intersects(targetBox);

            //assert
            result.Should().BeTrue();
        }

        [Test]
        public void When_BoxesAreDisjoint_ThenIntersectsIsFlase()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetBox = CreateBox(origin + 200, size);

            //act
            var result = sut.Intersects(targetBox);

            //assert
            result.Should().BeFalse();
        }

        [TestCase(100, true)]
        [TestCase(100.01f, false)]
        public void When_WhenSameSizeBoxesHaveConincidentSide_ThenIntersectsIsTrue(float distance, bool shouldBeConincident)
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetBox = CreateBox(origin + new Vector3(distance, 0, 0), size);

            //act
            var result = sut.Intersects(targetBox);

            //assert
            result.Should().Be(shouldBeConincident);
        }

        [TestCase(100, true)]
        [TestCase(100.01f, false)]
        public void When_WhenSameSizeBoxesHaveConincidentEdge_ThenIntersectsIsTrue(float distance, bool shouldBeConincident)
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetBox = CreateBox(origin + new Vector3(distance, distance, 0), size);

            //act
            var result = sut.Intersects(targetBox);

            //assert
            result.Should().Be(shouldBeConincident);
        }

        [Test]
        public void When_BoxBisectsBox_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetBox = CreateBox(origin, size + new Vector3(50, -50, 0));

            //act
            var result = sut.Intersects(targetBox);

            //assert
            result.Should().BeTrue();
        }

        [Test]
        public void When_BoxContainsSmallerSphere_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin, radius - 50);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().BeTrue();
        }

        [Test]
        public void When_BoxContainedInLargerSphere_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin, radius + 50);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().BeTrue();
        }

        [Test]
        public void When_BoxContainedInSameSizeSphere_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin, radius + 50);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().BeTrue();
        }

        [TestCase(100, true)]
        [TestCase(100.01f, false)]
        public void When_BoxSideAndSphereSurfaceAreCoincident_ThenIntersectsIsTrue(float distance, bool shouldBeCoincident)
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin + new Vector3(distance, 0, 0), radius);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().Be(shouldBeCoincident);
        }

        [TestCase(100f, true)]
        [TestCase(100.01f, false)]
        public void When_BoxEdgeAndSphereSurfaceAreCoincident_ThenIntersectsIsTrue(float distance, bool shouldBeCoincident)
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin + new Vector3(distance, distance/2.0f, 0), radius);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().Be(shouldBeCoincident);
        }

        [Test]
        public void When_BoxAndSpherePartiallyOverlap_ThenIntersectsIsTrue()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin + new Vector3(50, 50, 0), radius);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().BeTrue();
        }

        [Test]
        public void When_BoxAndSphereAreDisjoint_ThenIntersectsIsFalse()
        {
            //arrange
            const float size = 100f;
            const float radius = size / 2f;
            var origin = Vector3.Zero;
            var sut = CreateBox(origin, size);
            var targetSphere = new BoundingSphere(origin + 200, radius);

            //act
            var result = sut.Intersects(targetSphere);

            //assert
            result.Should().BeFalse();
        }

        public static BoundingBoxOA CreateBox(Vector3 origin, float size)
        {
            return CreateBox(origin, new Vector3(size));
        }

        public static BoundingBoxOA CreateBox(Vector3 origin, Vector3 size)
        {
            var halfSize = size / 2;
            var x = halfSize.X + origin.X;
            var y = halfSize.Y + origin.Y;
            var z = halfSize.Z + origin.Z;
            var _x = -halfSize.X + origin.X;
            var _y = -halfSize.Y + origin.Y;
            var _z = -halfSize.Z + origin.Z;
            return BoundingBoxOA.FromCorners(new Vector3[]
            {
                new Vector3(_x, y, z),
                new Vector3(x, y, z),
                new Vector3(x, _y, z),
                new Vector3(_x, _y, z),
                new Vector3(_x, y, _z),
                new Vector3(x, y, _z),
                new Vector3(x, _y, _z),
                new Vector3(_x, _y, _z),
            });
        }
    }
}