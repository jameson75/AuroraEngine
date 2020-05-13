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
        public BoundingQuadOA(Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft)
            : this()
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

            //Handle situations where the position of the ray exists on the quad's plane.
            if (Plane.DotCoordinate(plane, ray.Position) == 0)
            {
                //If the ray and quad are coplanar, we don't consider this an intersection.
                //Our rule in this game engine is that quads cannot be intersected by coplanar quads nor coplanar edges.
                if (Plane.DotNormal(plane, ray.Direction) == 0)
                {
                    point = Vector3.Zero;
                    return false;
                }
                //The ray's starting point exists on the quad's plane but the ray isn't coplanar.
                //We want to consider this to be an intersection. However, I believe that the 
                //sharpdx library doesn't recognize this as an intersection.
                //Therefore, we explicitly handle this case.
                else
                {
                    point = ray.Position;
                    return ContainsCoplanar(ref point);
                }
            }
            else               
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
                TopLeft,
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
            Vector3[] quad_corners = quad.GetCorners();
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
        public bool ContainsCoplanar(ref Vector3 coplanarPoint)
        {
            Plane plane = GetPlane();
            Vector3[] _corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
            {                
                Vector3 p1 = coplanarPoint;
                Vector3 p2 = _corners[i];
                Vector3 p3 = i < _corners.Length - 1 ? _corners[i + 1] : _corners[0];
                Vector3 c = Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p2));
                //CollisionDebugWriter.QuadContainsCoplanarPointInfo(c, plane, coplanarPoint);
                if (c != Vector3.Normalize(plane.Normal) && c != Vector3.Zero)
                    return false;                
            }
            //CollisionDebugWriter.QuadContainsCoplanarSuccessInfo();
            return true;
        }

        public BoundingQuadOA Transform(Matrix m)
        {
            return new BoundingQuadOA()
            {
                BottomLeft = Vector3.TransformCoordinate(this.BottomLeft, m),
                BottomRight = Vector3.TransformCoordinate(BottomRight, m),
                TopLeft = Vector3.TransformCoordinate(TopLeft, m),
                TopRight = Vector3.TransformCoordinate(TopRight, m),
            };
        }

        public override string ToString()
        {
            return string.Format("TopLeft: {0}, TopRight: {1}, BottomRight: {2}, BottomLeft: {3}",
                                  TopLeft, TopRight, BottomRight, BottomLeft);
        }

        public static BoundingQuadOA FromBox(BoundingBox boundingBox)
        {
            return new BoundingQuadOA()
            {
                TopLeft = new Vector3(boundingBox.Minimum.X, 0, boundingBox.Maximum.Z),
                TopRight = new Vector3(boundingBox.Maximum.X, 0, boundingBox.Maximum.Z),
                BottomRight = new Vector3(boundingBox.Maximum.X, 0, boundingBox.Minimum.Z),
                BottomLeft = new Vector3(boundingBox.Minimum.X, 0, boundingBox.Minimum.Z)
            };
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
