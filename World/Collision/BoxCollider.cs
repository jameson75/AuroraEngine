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
    public class BoxCollider : Collider
    {
        public BoundingBoxOA Box { get; set; }

        public bool EnableFastMovingObjectDetection { get; set; }

        public override CollisionEvent DetectCollision(Vector3 displacementVector, Collider targetCollider, Vector3 targetColliderDisplacementVector)
        {
            CollisionEvent collisionEvent = null;
            if (EnableFastMovingObjectDetection)
            {
                if (targetCollider is SphereCollider)
                {

                }
                else if (targetCollider is QuadCollider)
                {

                }
                else if (targetCollider is BoxCollider)
                {
                    Transform this_PreviousTransform = this.PreviousWorldTransform(displacementVector);
                    Transform targetCollider_PreviousTransform = targetCollider.PreviousWorldTransform(targetColliderDisplacementVector);

                    BoundingBoxOA wboxA = Box.Transform(this_PreviousTransform.ToMatrix());
                    Vector3 vectorA = displacementVector;

                    BoundingBoxOA wboxB = ((BoxCollider)targetCollider).Box.Transform(targetCollider_PreviousTransform.ToMatrix());
                    Vector3 vectorB = targetColliderDisplacementVector;

                    //Calculate the movement vector for A relative to B. (ie: in B's frame of reference).
                    Vector3 vectorArB = vectorA - vectorB;
                    Vector3 normalArB = Vector3.Normalize(vectorArB);
                    float? closestDistanceToContactArB = null;

                    //CollisionDebugWriter.ClearBufferedOut();
                    Vector3[] boxACorners = wboxA.GetCorners();
                    for (int i = 0; i < boxACorners.Length; i++)
                    {                       
                        Ray rayBoxCornerArB = new Ray(boxACorners[i], normalArB);
                        Vector3 contactPoint = Vector3.Zero;
                        //CollisionDebugWriter.BoxCornerInfo(i, boxACorners[i], normalArB);
                        if (wboxB.Intersects(ref rayBoxCornerArB, out contactPoint))
                        {
                            float distanceToContactSquared = Vector3.DistanceSquared(rayBoxCornerArB.Position, contactPoint);
                            if (distanceToContactSquared <= vectorArB.LengthSquared())
                            {
                                if (closestDistanceToContactArB == null || distanceToContactSquared < closestDistanceToContactArB.Value)
                                    closestDistanceToContactArB = distanceToContactSquared;
                            }
                        }
                    }

                    Vector3 vectorBrA = vectorB - vectorA;
                    Vector3 normalBrA = Vector3.Normalize(vectorBrA);
                    float? closestDistanceToContactBrA = null;

                    Vector3[] boxBCorners = wboxB.GetCorners();
                    for (int i = 0; i < boxBCorners.Length; i++)
                    {
                        Ray rayBoxCornerBrA = new Ray(boxBCorners[i], normalBrA);
                        Vector3 contactPoint = Vector3.Zero;
                        if (wboxA.Intersects(ref rayBoxCornerBrA, out contactPoint))
                        {
                            float distanceToContactSquared = Vector3.DistanceSquared(rayBoxCornerBrA.Position, contactPoint);
                            if (distanceToContactSquared <= vectorBrA.LengthSquared())
                            {
                                if (closestDistanceToContactBrA == null || distanceToContactSquared < closestDistanceToContactBrA.Value)
                                    closestDistanceToContactBrA = distanceToContactSquared;
                            }
                        }
                    }

                    if (closestDistanceToContactArB != null && closestDistanceToContactBrA.IsNullOrGreaterThanOrEqualTo(closestDistanceToContactArB.Value))
                    {
                        float distanceToCollisionA = closestDistanceToContactArB.Value;
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

                    else if (closestDistanceToContactBrA != null)
                    {
                        float distanceToCollisionB = closestDistanceToContactBrA.Value;
                        Vector3 collisionPointB = targetCollider_PreviousTransform.Translation + Vector3.Normalize(vectorB) * distanceToCollisionB;
                        float stepPercentageToCollision = distanceToCollisionB / vectorB.Length();
                        float distanceToCollisionA = vectorA.Length() * stepPercentageToCollision;
                        Vector3 collisionPointA = this_PreviousTransform.Translation + Vector3.Normalize(vectorA) * distanceToCollisionA;
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
            return collisionEvent;
        }
    }
}
