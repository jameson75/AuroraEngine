using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    public class BoxCollider : Collider
    {
        public BoundingBoxOA Box { get; set; }       

        public override CollisionEvent DetectCollision(ITransformable transformableContainer, Collider targetCollider, ITransformable targetTransformableContainer)
        {
            var collisionDetected = false;
            var worldBoxA = transformableContainer?.LocalToWorldBoundingBox(Box) ??
                               Box;

            if (targetCollider is SphereCollider)
            {
                var worldSphereB = targetTransformableContainer?.LocalToWorldBoundingSphere(((SphereCollider)targetCollider).Sphere) ??
                                   ((SphereCollider)targetCollider).Sphere;
                collisionDetected = worldBoxA.Intersects(worldSphereB);
            }

            else if (targetCollider is BoxCollider)
            {
                var worldBoxB = targetTransformableContainer?.LocalToWorldBoundingBox(((BoxCollider)targetCollider).Box) ??
                                ((BoxCollider)targetCollider).Box;
                collisionDetected = worldBoxB.Intersects(worldBoxA);
            }

            if (collisionDetected)
            {
                return new CollisionEvent()
                {
                    Collider1 = this,
                    Collider2 = targetCollider,
                    TransformableContainer1 = transformableContainer,
                    TransformableContainer2 = targetTransformableContainer
                };
            }

            return null;
        }
    }
}
