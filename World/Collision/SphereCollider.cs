using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Collision
{

    /// <summary>
    /// 
    /// </summary>
    public class SphereCollider : Collider
    {
        public BoundingSphere Sphere { get; set; }
        public bool EnableFastMovingObjectDetection { get; set; }

        public override CollisionEvent DetectCollision(Vector3 displacementVector, Collider targetCollider, Vector3 targetColliderDisplacementVector)
        {
            CollisionEvent collisionEvent = null;

            if (EnableFastMovingObjectDetection)
            {
                //Fast moving object collision detection
                //--------------------------------------

                if (targetCollider is SphereCollider)
                {
                    //*****************************************************************************************
                    //The following technique was dervied from this tutorial.
                    //http://www.gamasutra.com/view/feature/131424/pool_hall_lessons_fast_accurate_.php?page=2
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

                    Transform this_PreviousTransform = this.PreviousWorldTransform(displacementVector);
                    Transform targetCollider_PreviousTransform = targetCollider.PreviousWorldTransform(targetColliderDisplacementVector);

                    BoundingSphere sphereA = Sphere;
                    Vector3 vectorA = displacementVector;

                    BoundingSphere sphereB = ((SphereCollider)targetCollider).Sphere;
                    Vector3 vectorB = targetColliderDisplacementVector;

                    //Calculate the movement vector for A relative to B. (ie: in B's frame of reference).
                    Vector3 vectorArB = vectorA - vectorB;
                    Vector3 normalArB = Vector3.Normalize(vectorArB);

                    //Get the direction vector of A to B, as well as the normal from A to B.
                    Vector3 vectorC = targetCollider_PreviousTransform.Translation - this_PreviousTransform.Translation;
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
                                    Vector3 collisionPointA = this_PreviousTransform.Translation + Vector3.Normalize(vectorA) * distanceToCollisionA;
                                    float stepPercentageToCollision = distanceToCollisionA / vectorA.Length();
                                    float distanceToCollisionB = vectorB.Length() * stepPercentageToCollision;
                                    Vector3 collisionPointB = targetCollider_PreviousTransform.Translation + Vector3.Normalize(vectorB) * distanceToCollisionB;
                                    collisionEvent = new CollisionEvent()
                                    {
                                        Object1 = this,
                                        Object2 = targetCollider,
                                        Object1LocationAtCollision = collisionPointA,
                                        Object2LocationAtCollision = collisionPointB
                                    };
                                }
                            }
                        }
                    }
                }
                else if (targetCollider is QuadCollider)
                {
                    Transform this_PreviousTransform = this.PreviousWorldTransform(displacementVector);
                    Transform targetCollider_PreviousTransform = targetCollider.PreviousWorldTransform(targetColliderDisplacementVector);

                    BoundingSphere sphereA = Sphere;
                    Vector3 vectorA = displacementVector;
                    Vector3 vectorB = targetColliderDisplacementVector;

                    //Calculate the movement vector for A relative to B. (ie: in B's frame of reference).
                    Vector3 vectorArB = vectorA - vectorB;
                    Vector3 normalArB = Vector3.Normalize(vectorArB);

                    //Calculate the closest point on the sphere to the plane.
                    //This point will be the point that collides with the plane (assuming the plane nor sphere have rotational velocities).                    
                    Plane p = Plane.Transform(((QuadCollider)targetCollider).Quad.GetPlane(), targetCollider.WorldTransform().ToMatrix());
                    BoundingSphere s = new BoundingSphere(Vector3.TransformCoordinate(sphereA.Center, this.WorldTransform().ToMatrix()), sphereA.Radius);
                    Vector3 c = s.Center;
                    Vector3 closestPointOnPlane = Vector3.Zero;
                    Vector3 closestPointOnSphere = Vector3.Zero;
                    SharpDX.Collision.ClosestPointPlanePoint(ref p, ref c, out closestPointOnPlane);
                    SharpDX.Collision.ClosestPointSpherePoint(ref sphereA, ref closestPointOnPlane, out closestPointOnSphere);

                    //Calculate the point where the sphere will make contact with the plane, if at all.
                    Ray rayD = new Ray(closestPointOnSphere, normalArB);
                    Vector3 contactPoint = Vector3.Zero;
                    if (SharpDX.Collision.RayIntersectsPlane(ref rayD, ref p, out contactPoint))
                    {
                        //if we're not using the quads entire plane as a collision boundary,
                        //make sure the [co-planar] contact point exists within the quad's borders.
                        if (((QuadCollider)targetCollider).UseEntirePlane || ((QuadCollider)targetCollider).Quad.ContainsCoplanar(ref contactPoint))
                        {
                            //Calculate the distance to collision.
                            float distanceToCollisionA = (contactPoint - closestPointOnSphere).Length();

                            //If the collision will occur this frame then we generate a collision event for it.
                            if (distanceToCollisionA >= vectorArB.Length())
                            {
                                Vector3 collisionPointA = this.WorldTransform().Translation + Vector3.Normalize(vectorA) * distanceToCollisionA;
                                float stepPercentageToCollision = distanceToCollisionA / vectorA.Length();
                                float distanceToCollisionB = vectorB.Length() * stepPercentageToCollision;
                                Vector3 collisionPointB = targetCollider.WorldTransform().Translation + Vector3.Normalize(vectorB) * distanceToCollisionB;
                                collisionEvent = new CollisionEvent()
                                {
                                    Object1 = this,
                                    Object2 = targetCollider,
                                    Object1LocationAtCollision = collisionPointA,
                                    Object2LocationAtCollision = collisionPointB
                                };
                            }
                        }
                    }
                }
                else
                {
                    //NOTE: Right now, 
                    //We Can only detect fast moving object collisions into another sphere collider.
                }
            }
            else
            {
                //TODO: Determine whether the spheres intersect. 
                //If they intersect, determine the contact point and their respective locations (in world-space).
            }

            return collisionEvent;
        }
    }
}
