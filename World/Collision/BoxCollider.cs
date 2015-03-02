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
                    #region Obsolete
                    /*
                    
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
                    */
                    #endregion

                    Transform this_PreviousTransform = this.PreviousWorldTransform(displacementVector);
                    Transform targetCollider_PreviousTransform = targetCollider.PreviousWorldTransform(targetColliderDisplacementVector);

                    BoundingBoxOA wboxA = Box.Transform(this_PreviousTransform.ToMatrix());
                    Vector3 vectorA = displacementVector;

                    BoundingBoxOA wboxB = ((BoxCollider)targetCollider).Box.Transform(targetCollider_PreviousTransform.ToMatrix());
                    Vector3 vectorB = targetColliderDisplacementVector;

                    //Calculate the movement vector for A relative to B. (ie: in B's frame of reference).
                    Vector3 vectorArB = vectorA - vectorB;
                    
                    float? closestDistanceToContactArB = null;
                    GetClosestDistanceToCollision(wboxA, wboxB, vectorArB, out closestDistanceToContactArB);
                    
                    float? closestDistanceToContactBrA = null;
                    GetClosestDistanceToCollision(wboxB, wboxA, -vectorArB, out closestDistanceToContactBrA);

                    //If an actual collision event was found from out of all the potential collisions, create and return a collision event.
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

        private void GetClosestDistanceToCollision(BoundingBoxOA wboxA, BoundingBoxOA wboxB, Vector3 movementVector, out float? closestDistanceToContact)
        {
            BoundingQuadOA[] wQuadsA = wboxA.GetQuads();
            BoundingQuadOA[] wQuadsB = wboxB.GetQuads();
            Vector3 movementNormal = Vector3.Normalize(movementVector);
            List<Vector3> potentialCollisionPoints = new List<Vector3>();

            List<Vector3> debug = new List<Vector3>();

            foreach (BoundingQuadOA wQuadA in wQuadsA)
            {
                Vector3[] wQuadPointsA = wQuadA.GetCorners();
                foreach (BoundingQuadOA wQuadB in wQuadsB)
                {
                    //Get Projected-Overlapping-Poly
                    //------------------------------                            
                    Vector3[] wQuadPointsB = wQuadB.GetCorners();
                    if (wQuadA.CanProjectOnTo(wQuadB, movementNormal))
                    {
                        //Project quad A onto the surface of quad B along A's (relative) momentum vector and aquire it's corners.
                        BoundingQuadOA pQuadA = wQuadA.ProjectOnTo(wQuadB, movementNormal);
                        Vector3[] pQuadPointsA = pQuadA.GetCorners();                        

                        //We now use the intersection of projected quad A and quad B to determine all posible collision points.
                        for (int i = 0; i < pQuadPointsA.Length; i++)
                        {
                            Vector3 pEdgePointA1 = pQuadPointsA[i];
                            Vector3 pEdgePointA2 = pQuadPointsA.NextOrFirst(i);

                            //If the point of the projected quad, A, exists in the interior of quad B, then it is a potential collision point.
                            if (wQuadB.ContainsCoplanar(ref pEdgePointA1) && !potentialCollisionPoints.Contains(pEdgePointA1))
                                potentialCollisionPoints.Add(pEdgePointA1);

                            //Every edge-intersection between projected quad, A, and quad B is an potential collision point.                          
                            for (int j = 0; j < wQuadPointsB.Length; j++)
                            {
                                Vector3 wEdgePointB1 = wQuadPointsB[i];
                                Vector3 wEdgePointB2 = pQuadPointsA.NextOrFirst(i);
                                Vector3 intersection = Vector3.Zero;
                                if (CollisionExtension.LineIntersectLine(pEdgePointA1, pEdgePointA2, wEdgePointB1, wEdgePointB2, out intersection) &&
                                    intersection != Vector3.Zero &&
                                    !potentialCollisionPoints.Contains(intersection))
                                {
                                    potentialCollisionPoints.Add(intersection);
                                    debug.Add(intersection);
                                }
                            }                                                       
                        }

                        //If the point of the projected quad, A, exists in the interior of quad B, then it is a potential collision point.                               
                        for (int i = 0; i < wQuadPointsB.Length; i++)
                        {
                            Vector3 wQuadPointB = wQuadPointsB[i];
                            if (pQuadA.ContainsCoplanar(ref wQuadPointB) && !potentialCollisionPoints.Contains(wQuadPointB))
                                potentialCollisionPoints.Add(wQuadPointB);
                        }
                    }
                }
            }

            //Shoot rays from the all potential collision points to quadA, tracking the distance of the ray with the shortest
            //distance to intersection (if any).
            //CollisionDebugWriter.ClearBufferedOut();
            closestDistanceToContact = null;
            foreach (Vector3 potentialCollisionPoint in potentialCollisionPoints)
            {
                Ray rayFromPotentialCollisionPoint = new Ray(potentialCollisionPoint, -movementNormal);
                Vector3 contactPoint = Vector3.Zero;
                //CollisionDebugWriter.BoxCornerInfo(i, polyPoint, normalArB);
                if (wboxA.Intersects(ref rayFromPotentialCollisionPoint, out contactPoint))
                {
                    float distanceToContactSquared = Vector3.DistanceSquared(rayFromPotentialCollisionPoint.Position, contactPoint);
                    if (distanceToContactSquared <= movementVector.LengthSquared())
                    {
                        if (closestDistanceToContact == null || distanceToContactSquared < closestDistanceToContact.Value)
                            closestDistanceToContact = distanceToContactSquared;
                    }
                }
            }
        }
    }

    public static class _IEnumerableExtension
    {
        public static T NextOrFirst<T>(this IEnumerable<T> e, int i)
        {
            return (i < e.Count() - i) ? e.ElementAt(i) : e.FirstOrDefault();
        }
    }

    public static class BoundingQuadOAExtension
    {
        public static bool CanProjectOnTo(this BoundingQuadOA source, BoundingQuadOA target, Vector3 direction)
        {
            //We consider the source "projectable" on to the target only if target's plane faces
            //all corners of the source quad.
            Vector3[] corners = source.GetCorners();
            Plane targetPlane = target.GetPlane();
            for (int i = 0; i < corners.Length; i++)
            {
                if (Plane.DotCoordinate(targetPlane, corners[i]) <= 0)
                    return false;

                if (new Ray(corners[i], direction).Intersects(ref targetPlane) == false)
                    return false;
            }
            return true;
        }

        public static BoundingQuadOA ProjectOnTo(this BoundingQuadOA source, BoundingQuadOA target, Vector3 direction)
        {
            Vector3[] projectedCorners = source.GetCorners()
                                               .Select(p => target.GetPlane().ProjectPoint(p, direction).Value)
                                               .ToArray();
            return BoundingQuadOA.FromPoints(projectedCorners);
        }
    }
}
