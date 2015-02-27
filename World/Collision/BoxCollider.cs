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
                    Vector3 normalArB = Vector3.Normalize(vectorArB);

                    BoundingQuadOA[] wQuadsA = wboxA.GetQuads();
                    BoundingQuadOA[] wQuadsB = wboxB.GetQuads();

                    foreach (BoundingQuadOA wQuadA in wQuadsA)
                    {
                        Vector3[] wQuadPointsA = wQuadA.GetCorners();                        
                        foreach (BoundingQuadOA wQuadB in wQuadsB)
                        {                           
                            //Get Projected-Overlapping-Poly
                            //------------------------------

                            //Acquire the corners of quad B.
                            Vector3[] wQuadPointsB = wQuadB.GetCorners();

                            //Project quad A onto the surface of quad B along A's (relative) momentum vector and aquire it's corners.
                            Vector3[] pQuadPointsA = wQuadPointsA.Select(p => wQuadB.GetPlane().ProjectPoint(p, normalArB).Value).ToArray();

                            //We now use the intersection of projected quad A and quad B to determine all posible collision points.
                            List<Vector3> potentialCollisionPoints = new List<Vector3>();                            
                            for( int i = 0; i < pQuadPointsA.Length; i++)
                            {
                                Vector3 pEdgePointA1 = pQuadPointsA[i];
                                Vector3 pEdgePointA2 = pQuadPointsA.NextOrFirst(i);

                                //If the point of the projected quad, A, exists in the interior of quad B, then it is a potential collision point.
                                if (wQuadB.ContainsCoplanar(ref pEdgePointA1) && !potentialCollisionPoints.Contains(pEdgePointA1))
                                    potentialCollisionPoints.Add(pEdgePointA1);
                                
                                //Every edge-intersection between projected quad, A, and quad B is an potential collision point.
                                List <Vector3> intersectingPoints = new List<Vector3>();                                
                                for (int j = 0; j < wQuadPointsB.Length; j++)
                                {
                                    Vector3 wEdgePointB1 = wQuadPointsB[i];
                                    Vector3 wEdgePointB2 = pQuadPointsA.NextOrFirst(i);
                                    Vector3 intersection = Vector3.Zero;
                                    if (CollisionExtension.LineIntersectLine(pEdgePointA1, pEdgePointA2, wEdgePointB1, wEdgePointB2, out intersection) &&  !potentialCollisionPoints.Contains(intersection))                                                                                        
                                        intersectingPoints.Add(intersection);                                                                                                                    
                                }
                                //We ensure the intersections points detected between the current edge in projected quad A and all the edges in quad B
                                //are ordered closest-to-furthest. (It's possible that A and B are oriented in such a way that the intersection
                                //points were detected furthest-to-closest).
                                intersectingPoints.OrderBy(p => Vector3.DistanceSquared(pEdgePointA1, p));                                                               
                                potentialCollisionPoints.AddRange(intersectingPoints);
                            }

                            //If the point of the projected quad, A, exists in the interior of quad B, then it is a potential collision point.
                            BoundingQuadOA pQuadA = BoundingQuadOA.FromPoints(pQuadPointsA);
                            for(int i = 0; i < wQuadPointsB.Length; i++)
                            {
                                Vector3 wQuadPointB = wQuadPointsB[i];
                                if (pQuadA.ContainsCoplanar(ref wQuadPointB) && !potentialCollisionPoints.Contains(wQuadPointB))                            
                                    potentialCollisionPoints.Add(wQuadPointB);                                
                            }

                            //Shoot rays from the all potential collision points to quadA, tracking the distance of the ray with the shortest
                            //distance to intersection (if any).
                            //CollisionDebugWriter.ClearBufferedOut();
                            float? closestDistanceToContactArB = null;                          
                            foreach (Vector3 polyPoint in potentialCollisionPoints)
                            {
                                Ray rayPolyPointArB = new Ray(polyPoint, normalArB);
                                Vector3 contactPoint = Vector3.Zero;
                                //CollisionDebugWriter.BoxCornerInfo(i, polyPoint, normalArB);
                                if (wboxB.Intersects(ref rayPolyPointArB, out contactPoint))
                                {
                                    float distanceToContactSquared = Vector3.DistanceSquared(rayPolyPointArB.Position, contactPoint);
                                    if (distanceToContactSquared <= vectorArB.LengthSquared())
                                    {
                                        if (closestDistanceToContactArB == null || distanceToContactSquared < closestDistanceToContactArB.Value)
                                            closestDistanceToContactArB = distanceToContactSquared;
                                    }
                                }
                            }

                            //If an actual collision event was found from out of all the potential collisions, create and return a collision event.
                            if (closestDistanceToContactArB != null)
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
                        }                        
                    }              
                }
            }

            return collisionEvent;
        }
    }

    public static class _IEnumerableExtension
    {
        public static T NextOrFirst<T>(this IEnumerable<T> e, int i)
        {
            return (i < e.Count() - i) ? e.ElementAt(i) : e.FirstOrDefault();
        }
    }
}
