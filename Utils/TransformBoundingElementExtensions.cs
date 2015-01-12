using System;
using SharpDX;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World;

namespace CipherPark.AngelJacket.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class TransformableExtensionForBoundingElements
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public static BoundingSphere LocalToWorldBoundingSphere(this ITransformable transformable, BoundingSphere sphere)
        {
            return new BoundingSphere(transformable.WorldTransform().Translation, sphere.Radius);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingBox LocalToWorldBoundingBox(this ITransformable transformable, BoundingBox box)
        {
            Vector3 thisWorldPosition = transformable.WorldTransform().Translation;
            return new BoundingBox(thisWorldPosition + box.Minimum, thisWorldPosition + box.Maximum);
        }
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    public static class WorldObjectExtensionForBoundingElements
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wo"></param>
        /// <returns></returns>
        public static BoundingSphere WorldBoundingSphere(this WorldObject wo)
        {
            return wo.LocalToWorldBoundingSphere(wo.BoundingSphere);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wo"></param>
        /// <returns></returns>
        public static BoundingBox WorldBoundingBox(this WorldObject wo)
        {
            return wo.LocalToWorldBoundingBox(wo.BoundingBox);
        }
    }
    */
}
