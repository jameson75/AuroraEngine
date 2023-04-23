using System;
using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    public class SphereCollider : Collider, IFastMovingCollider
    {
        public BoundingSphere Sphere { get; set; }

        public bool IsFastMovingCollider { get; set; }

        public override CollisionEvent DetectCollision(
            ITransformable transformableContainer, 
            Collider targetCollider, 
            ITransformable targetTransformableContainer)
        {
            var collisionDetected = false;
            var worldSphereA = transformableContainer?.LocalToWorldBoundingSphere(Sphere) ?? 
                               Sphere;

            if (targetCollider is SphereCollider)
            {               
                var worldSphereB = targetTransformableContainer?.LocalToWorldBoundingSphere(((SphereCollider)targetCollider).Sphere) ?? 
                                   ((SphereCollider)targetCollider).Sphere;                
                collisionDetected = worldSphereA.Intersects(worldSphereB);
            }

            else if (targetCollider is BoxCollider)
            {
                var worldBoxB = targetTransformableContainer?.LocalToWorldBoundingBox(((BoxCollider)targetCollider).Box) ??
                                ((BoxCollider)targetCollider).Box;
                collisionDetected = worldBoxB.Intersects(worldSphereA);
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

        public PredictedCollisionEvent PredictCollision(
            ITransformable transformableContainer,
            Vector3 displacementVector,
            Collider targetCollider,
            ITransformable targetTransformableContainer,
            Vector3 targetColliderDisplacementVector)
        {
            PredictedCollisionEvent collisionEvent = null;

            if (targetCollider is SphereCollider)
            {
                //*****************************************************************************************
                //The following technique was dervied from this tutorial.
                //http://www.gamasutra.com/view/feature/131424/pool_hall_lessons_fast_accurate_.php?page=2
                //https://www.gamedeveloper.com/programming/pool-hall-lessons-fast-accurate-collision-detection-between-circles-or-spheres
                // Legend:
                // vectorA - movement vector for object A.
                // vectorB - movement vector for object B.
                // vectorC - vector from A to B.
                // lengthD - length along vectorA from object A to closest point to object B. 
                // vectorArB - movement vector for oject A in object B's reference frame.
                // lengthTSquared - 
                // lengthFSquared - 
                // radiiAB - 
                //*****************************************************************************************

                BoundingSphere sphereA = Sphere;
                Vector3 vectorA = displacementVector;

                BoundingSphere sphereB = ((SphereCollider)targetCollider).Sphere;
                Vector3 vectorB = targetColliderDisplacementVector;

                Vector3 positionColliderA = transformableContainer?.ParentToWorldCoordinate(sphereA.Center) ?? sphereA.Center;
                Vector3 positionColliderB = targetTransformableContainer?.ParentToWorldCoordinate(sphereB.Center) ?? sphereB.Center;

                //Calculate the movement vector for A relative to B. (ie: in B's frame of reference).
                Vector3 vectorArB = vectorA - vectorB;
                Vector3 normalArB = Vector3.Normalize(vectorArB);

                //Get the direction vector of A to B, as well as the normal from A to B.
                Vector3 vectorC = positionColliderB - positionColliderA;
                float lengthC = vectorC.Length();

                float radiiAB = (sphereA.Radius + sphereB.Radius);
                float radiiABSquared = radiiAB * radiiAB;

                //We only continue checking for a collision if the 
                //distance A is moving [relative to B] is greater or equal to 
                //the distance between A and B minus their radii. If it's not,
                //then there's no collision (this frame).
                if (vectorArB.Length() >= lengthC - radiiAB)
                {
                    //Get the signed length of the [relative-to-B] vector from object A to the closest co-linear point to object B.
                    float lengthD = Vector3.Dot(normalArB, vectorC);

                    //Check to see if A is actually moving towards or away from B.
                    //Only if A is approaching B, do we check for a collision between A and B.                        
                    if (lengthD > 0)
                    {
                        float lengthFSquared = (lengthC * lengthC) - (lengthD * lengthD);

                        // If the closest that A will get to B 
                        // is more than the sum of their radii, there's no 
                        // way they are going collide                          
                        if (lengthFSquared < radiiABSquared)
                        {
                            float lengthTSquared = radiiABSquared - lengthFSquared;
                            if (lengthTSquared > 0)
                            {
                                float distanceToCollisionA = lengthD - (float)Math.Sqrt(lengthTSquared);
                                Vector3 locationOfCollisionA = positionColliderA + Vector3.Normalize(vectorA) * distanceToCollisionA;
                                float stepPercentageToCollision = distanceToCollisionA / vectorA.Length();
                                float distanceToCollisionB = vectorB.Length() * stepPercentageToCollision;
                                Vector3 locationOfCollisionB = positionColliderB + Vector3.Normalize(vectorB) * distanceToCollisionB;
                                var sphereAtCollisionB = new BoundingSphere(positionColliderB, sphereB.Radius);
                                var rayABAtCollision = new Ray(positionColliderA, Vector3.Normalize(positionColliderB - positionColliderA));
                                sphereAtCollisionB.Intersects(ref rayABAtCollision, out Vector3 intersectionPoint);
                                collisionEvent = new PredictedCollisionEvent()
                                {
                                    Collider1 = this,
                                    Collider2 = targetCollider,
                                    TransformableContainer1 = transformableContainer,
                                    TransformableContainer2 = targetTransformableContainer,
                                    PredictedCollisionLocation1 = locationOfCollisionA,
                                    PredictedCollisionLocation2 = locationOfCollisionB,                                    
                                };
                            }
                        }
                    }
                }
            }
            return collisionEvent;
        }
    }
}
