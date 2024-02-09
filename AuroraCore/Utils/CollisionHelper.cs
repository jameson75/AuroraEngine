using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Utils
{
    public static class CollisionHelper
    {
        public static bool LineIntersectLine(Vector3 pA1, Vector3 pA2, Vector3 pB1, Vector3 pB2, out Vector3 intersection)
        {
            Ray rA = new Ray(pA1, Vector3.Normalize(pA2 - pA1));
            Ray rB = new Ray(pB1, Vector3.Normalize(pB2 - pB1));
       
            //NOTE: When the two rays are coincident, the sharpdx library returns
            //true, and returns an intersection of Vector3.Zero. 
            //This is to ambiguous for our purposes.
            //As a rule in this engine, neither conicident rays nor lines are considered
            //to be intersecting,  since we cannot define a single point of intersection.    
                   
            //if (rA.Intersects(ref rB, out intersection) && !AreRaysCoincident(rA, rB))
            if (RayIntersectsRay(ref rA, ref rB, out intersection) == RayRayIntersectionResult.Intersect)
            {               
                float dA = Vector3.DistanceSquared(pA1, pA2);
                float dB = Vector3.DistanceSquared(pB1, pB2);
                float dAI = Vector3.DistanceSquared(pA1, intersection);
                float dBI = Vector3.DistanceSquared(pB1, intersection);
                if (dAI <= dA && dBI <= dB)
                    return true;
            }
            intersection = Vector3.Zero;
            return false;
        }       

        public static RayRayIntersectionResult RayIntersectsRay(ref Ray ray1, ref Ray ray2, out Vector3 point) 
        { 
            //Source: Real-Time Rendering, Third Edition 
            //Reference: Page 780  
             Vector3 cross; 
 
             Vector3.Cross(ref ray1.Direction, ref ray2.Direction, out cross); 
             float denominator = cross.Length(); 
 
             //Lines are parallel. 
             if (MathUtil.IsZero(denominator)) 
             { 
                 //Lines are parallel and on top of each other. 
                 if (MathUtil.NearEqual(ray2.Position.X, ray1.Position.X) && 
                     MathUtil.NearEqual(ray2.Position.Y, ray1.Position.Y) && 
                     MathUtil.NearEqual(ray2.Position.Z, ray1.Position.Z)) 
                 { 
                     point = Vector3.Zero; 
                     return RayRayIntersectionResult.Overlap; 
                 } 
             }  
 
             denominator = denominator * denominator;  
 
             //3x3 matrix for the first ray. 
             float m11 = ray2.Position.X - ray1.Position.X; 
             float m12 = ray2.Position.Y - ray1.Position.Y; 
             float m13 = ray2.Position.Z - ray1.Position.Z; 
             float m21 = ray2.Direction.X; 
             float m22 = ray2.Direction.Y; 
             float m23 = ray2.Direction.Z; 
             float m31 = cross.X; 
             float m32 = cross.Y; 
             float m33 = cross.Z;  
 
             //Determinant of first matrix. 
             float dets = 
                 m11 * m22 * m33 + 
                 m12 * m23 * m31 + 
                 m13 * m21 * m32 - 
                 m11 * m23 * m32 - 
                 m12 * m21 * m33 - 
                 m13 * m22 * m31;  
 
             //3x3 matrix for the second ray. 
             m21 = ray1.Direction.X; 
             m22 = ray1.Direction.Y; 
             m23 = ray1.Direction.Z;  
 
             //Determinant of the second matrix. 
             float dett = 
                 m11 * m22 * m33 + 
                 m12 * m23 * m31 + 
                 m13 * m21 * m32 - 
                 m11 * m23 * m32 - 
                 m12 * m21 * m33 - 
                 m13 * m22 * m31;  
 
             //t values of the point of intersection. 
             float s = dets / denominator; 
             float t = dett / denominator;

             if (float.IsNaN(s) || float.IsNaN(t))
             {
                 point = Vector3.Zero;
                 return RayRayIntersectionResult.None;
             }

             //The points of intersection. 
             Vector3 point1 = ray1.Position + (Math.Max(s, 0) * ray1.Direction); 
             Vector3 point2 = ray2.Position + (Math.Max(t, 0) * ray2.Direction);  
 
             //If the points are not equal, no intersection has occurred. 
             if (!MathUtil.NearEqual(point2.X, point1.X) || 
                 !MathUtil.NearEqual(point2.Y, point1.Y) || 
                 !MathUtil.NearEqual(point2.Z, point1.Z)) 
             { 
                 point = Vector3.Zero; 
                 return RayRayIntersectionResult.None; 
             }  
 
             point = point1;
             return RayRayIntersectionResult.Intersect;
         } 
    }
}
