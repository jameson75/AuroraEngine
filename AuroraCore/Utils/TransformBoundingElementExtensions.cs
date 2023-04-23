using SharpDX;
using CipherPark.Aurora.Core.Animation;

namespace CipherPark.Aurora.Core.Utils
{
    /// <summary>
    /// </summary>
    public static class TransformableExtensionForBoundingElements
    {
        /// <summary>
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingBoxOA LocalToWorldBoundingBox(this ITransformable transformable, BoundingBox box)
        {
            var boundingBoxOA = BoundingBoxOA.FromAABoundingBox(box);
            return LocalToWorldBoundingBox(transformable, boundingBoxOA);
        }

        /// <summary>
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingBoxOA LocalToWorldBoundingBox(this ITransformable transformable, BoundingBoxOA box)
        {
            return box.Transform(transformable.WorldTransform().ToMatrix());
        }

        /// <summary>
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingSphere LocalToWorldBoundingSphere(this ITransformable transformable, BoundingSphere sphere)
        {
            return new BoundingSphere(transformable.WorldToLocalCoordinate(sphere.Center), sphere.Radius);
        }
    }
}
