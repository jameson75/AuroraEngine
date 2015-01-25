using System;
using System.Linq;
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
        public static BoundingBoxOA BoundingBoxOA(this WorldObject wo)
        {
            Vector3[] corners = wo.BoundingBox.GetCorners().Select(c => wo.LocalToWorldCoordinate(c)).ToArray();
            return Utils.BoundingBoxOA.FromPoints(corners);
        }
    }

    /// <summary>
    /// Represents and object aligned bounding box.
    /// </summary>
    public struct BoundingBoxOA
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackTopLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackTopRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackBottomRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackBottomLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontTopLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontTopRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontBottomRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontBottomLeft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static BoundingBoxOA FromPoints(Vector3[] points)
        {
            //******************************************************
            //NOTE:  
            //ASSUMING Left-Handed system, clockwise winding order.
            //If viewer is facing +z direction then...
            //back 0, 1, 2, 3 - starting from top left.
            //front 4, 5, 6, 7 - starting from top left.
            //******************************************************
            BoundingBoxOA newBox = new BoundingBoxOA();
            newBox.BackTopLeft = points[0];
            newBox.BackTopRight = points[1];
            newBox.BackBottomRight = points[2];
            newBox.BackBottomLeft = points[3];
            newBox.FrontTopLeft = points[4];
            newBox.FrontTopRight = points[5];
            newBox.FrontBottomRight = points[6];
            newBox.FrontBottomLeft = points[7];
            return newBox;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public ContainmentType Contains(ref BoundingBox box)
        {
            int nCornersContained = 0;

            Vector3[] _corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
            {
                Vector3 c = _corners[i];
                ContainmentType t = box.Contains(ref c);                
                if (t == ContainmentType.Intersects)
                    return ContainmentType.Intersects;                
                else if (t == ContainmentType.Contains)
                    nCornersContained++;               
            }

            if (nCornersContained == 0)
                return ContainmentType.Disjoint;
            else if (nCornersContained == _corners.Length)
                return ContainmentType.Contains;
            else
                return ContainmentType.Intersects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public ContainmentType Contains(ref Vector3 point)
        {
            return Contains(ref point, GetPlanes());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public ContainmentType Contains(ref BoundingSphere sphere)
        {
            Plane[] planes = GetPlanes();
            for (int i = 0; i < planes.Length; i++)
            {
                if (sphere.Intersects(ref planes[i]) == PlaneIntersectionType.Intersecting)
                    return ContainmentType.Intersects;
            }
            return this.Contains(ref sphere.Center, planes);          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool Intersects(ref BoundingSphere sphere)
        {
            return this.Contains(ref sphere) != ContainmentType.Disjoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public PlaneIntersectionType Intersects(ref Plane plane)
        {
            bool hasPointInFront = false;
            bool hasPointInBack = false;

            Vector3[] _corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
            {
                PlaneIntersectionType t = plane.Intersects(ref _corners[i]);
                if (t == PlaneIntersectionType.Front)
                    hasPointInFront = true;
                else if (t == PlaneIntersectionType.Back)
                    hasPointInBack = true;

                //Return 'Intersecting' if any point of this bounding box is coplanar to the specified
                //plane or we've determined that this bounding box has points on both sides of that plane.
                if (t == PlaneIntersectionType.Intersecting || (hasPointInFront && hasPointInBack))
                    return PlaneIntersectionType.Intersecting;
            }

            //At this point, it's been determined that all corners of this bounding box exist on the
            //same side of the specified plane.

            return hasPointInFront ? PlaneIntersectionType.Front : PlaneIntersectionType.Back;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Intersects(ref Ray ray, out Vector3 point)
        {
            BoundingQuadOA[] quads = GetQuads();
            point = Vector3.Zero;
            for (int i = 0; i < quads.Length; i++)
            {
                Plane plane = quads[i].GetPlane();
               
                //We discard any planes of this bounding box which are facing away from the ray's origin.                
                if (Plane.DotCoordinate(plane, point) >= 0)
                {
                    //NOTE: At most, only one plane of the bounding box, that' either facing or coplanar to the ray's origin,
                    //can be intersected.

                    //We return the intersection point of the first plane intersected.
                    if (quads[i].Intersects(ray, out point))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planes"></param>
        /// <returns></returns>
        private ContainmentType Contains(ref Vector3 point, Plane[] planes)
        {
            bool intersectsAPlane = false;
            for (int i = 0; i < planes.Length; i++)
            {
                PlaneIntersectionType t = planes[i].Intersects(ref point);
                
                //If the point is in front of any plane of the bounding box
                //then it is on the outside of the box. Otherwise, it's either
                //inside the box or on the surface of the box (intersects the box)>
                if (t == PlaneIntersectionType.Front)
                    return ContainmentType.Disjoint;
                
                else if (t == PlaneIntersectionType.Intersecting)
                    intersectsAPlane = true;
            }

            //At this point we know that the point is either inside or
            //on the surface (intersects a plane) of the of the bounding box.

            return intersectsAPlane ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is BoundingBoxOA)
                return Equals((BoundingBoxOA)obj);
            else
                return false;         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Equals(BoundingBoxOA box)
        {
            Vector3[] _corners = GetCorners();
            Vector3[] box_corners = box.GetCorners();
            for (int i = 0; i < _corners.Length; i++)
                if (_corners[i] != box_corners[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns planes in the order of Front, Back, Top, Bottom, Left, Right.
        /// </summary>
        /// <returns></returns>
        public Plane[] GetPlanes()
        { 
            return GetQuads().Select(q => q.GetPlane()).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BoundingQuadOA[] GetQuads()
        {
            //******************************************************
            //NOTE:  
            //ASSUMING Left-Handed system, clockwise winding order.
            //If viewer is facing +z direction then...
            //back 0, 1, 2, 3 - starting from top left.
            //front 4, 5, 6, 7 - starting from top left.
            //******************************************************
            Vector3[] _corners = GetCorners();
            BoundingQuadOA front, back, top, bottom, left, right;
            front =     new BoundingQuadOA(_corners[4], _corners[5], _corners[6], _corners[7]); 
            back =      new BoundingQuadOA(_corners[0], _corners[3], _corners[2], _corners[1]);  
            top =       new BoundingQuadOA(_corners[0], _corners[1], _corners[5], _corners[4]);  
            bottom =    new BoundingQuadOA(_corners[3], _corners[7], _corners[6], _corners[2]); 
            left =      new BoundingQuadOA(_corners[0], _corners[4], _corners[7], _corners[3]); 
            right =     new BoundingQuadOA(_corners[5], _corners[1], _corners[2], _corners[6]);
            return new BoundingQuadOA[] { front, back, top, bottom, left, right };
        }

        public Vector3[] GetCorners()
        {
            //******************************************************
            //NOTE:  
            //ASSUMING Left-Handed system, clockwise winding order.
            //If viewer is facing +z direction then...
            //back 0, 1, 2, 3 - starting from top left.
            //front 4, 5, 6, 7 - starting from top left.
            //******************************************************
            return new Vector3[]
            {
                BackTopLeft,
                BackTopRight,
                BackBottomRight,
                BackBottomLeft,
                FrontTopLeft,
                FrontTopRight,
                FrontBottomRight,
                FrontBottomLeft
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct BoundingQuadOA
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 TopLeft { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 TopRight { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BottomRight { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BottomLeft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomRight"></param>
        /// <param name="bottomLeft"></param>
        public BoundingQuadOA(Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft) : this()
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static BoundingQuadOA FromPoints(Vector3[] points)
        {
            BoundingQuadOA quad = new BoundingQuadOA();           
            quad.TopLeft = points[0];
            quad.TopRight = points[1];
            quad.BottomRight = points[2];
            quad.BottomLeft = points[3];
            return quad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public QuadIntersectionType Intersects(ref Vector3 point)
        {
            Plane plane = GetPlane();
            PlaneIntersectionType t = plane.Intersects(ref point);
            switch (t)
            {
                case PlaneIntersectionType.Intersecting:
                    return this.ContainsCoplanar(ref point) ? QuadIntersectionType.Intersects : QuadIntersectionType.CoplanarExterior;
                case PlaneIntersectionType.Back:
                    return QuadIntersectionType.Back;
                case PlaneIntersectionType.Front:
                    return QuadIntersectionType.Front;
                default:
                    throw new InvalidOperationException("Unexpected intersection type.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Intersects(Ray ray, out Vector3 point)
        {
            Plane plane = GetPlane();           
            return ray.Intersects(ref plane, out point) && ContainsCoplanar(ref point);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Intersects(Ray ray, out float distance)
        {           
            Vector3 point;
            if (this.Intersects(ray, out point))
            {
                distance = Vector3.Distance(ray.Position, point);
                return true;
            }
            else
            {
                distance = 0;
                return false;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Intersects(Ray ray)
        {
            Vector3 point;
            return (this.Intersects(ray, out point));           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Plane GetPlane()
        {
            return new Plane(TopLeft, TopRight, BottomRight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetCorners()
        {
            return new Vector3[]
            {
                TopRight,
                TopRight,
                BottomRight,
                BottomLeft
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is BoundingQuadOA)
                return Equals((BoundingQuadOA)obj);
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quad"></param>
        /// <returns></returns>
        public bool Equals(BoundingQuadOA quad)
        {
            Vector3[] _corners = GetCorners();
            Vector3[] quad_corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
                if (_corners[i] != quad_corners[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coplanarPoint"></param>
        /// <returns></returns>
        private bool ContainsCoplanar(ref Vector3 coplanarPoint)
        {
            Plane plane = GetPlane();
            Vector3[] _corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
            {
                Vector3 p1 = coplanarPoint;
                Vector3 p2 = _corners[i];
                Vector3 p3 = i < _corners.Length - 1 ? _corners[i + 1] : _corners[0];
                Plane p = new Plane(p1, p2, p3);
                if (Vector3.Normalize(p.Normal) != Vector3.Normalize(plane.Normal))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum QuadIntersectionType
    {
        Back,
        Front,
        CoplanarExterior,
        Intersects
    }
}
