using CipherPark.Aurora.Core.Animation;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class FluentTransformExtensions
    {
        public static Transform Rotate(this Transform transform, Vector3 axis, float delta)
        {
            var qDelta = Quaternion.RotationAxis(axis, delta);
            return new Transform(qDelta * transform.Rotation, transform.Translation, transform.Scale);
        }

        public static Transform Translate(this Transform transform, Vector3 delta)
        {
            return new Transform(transform.Rotation, delta + transform.Translation, transform.Scale);
        }

        public static Transform RotateTo(this Transform transform, Vector3 axis, float angle)
        {
            var rotation = Quaternion.RotationAxis(axis, angle);
            return new Transform(rotation, transform.Translation, transform.Scale);
        }

        public static Transform TranslateTo(this Transform transform, Vector3 position)
        {
            return new Transform(transform.Rotation, position, transform.Scale);
        }
    }
}
