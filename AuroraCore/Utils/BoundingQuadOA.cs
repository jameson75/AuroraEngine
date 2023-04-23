using System;
using System.Linq;
using SharpDX;

///////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////

namespace CipherPark.Aurora.Core.Utils
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
        /// creates a bounding quad from points
        /// </summary>
        /// <remarks>
        /// expects points in this order: top-left, top-right, bottom-right, bottom-left.
        /// </remarks>
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
        public QuadIntersectionType Intersects(Vector3 point)
        {
            Plane plane = GetPlane();
            PlaneIntersectionType t = plane.Intersects(ref point);
            switch (t)
            {
                case PlaneIntersectionType.Intersecting:
                    return this.ContainsCoplanar(point) ? QuadIntersectionType.Intersects : QuadIntersectionType.CoplanarExterior;
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
                    return ContainsCoplanar(point);
                }
            }
            else               
                return ray.Intersects(ref plane, out point) && ContainsCoplanar(point);            
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
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool Intersects(ref BoundingSphere sphere)
        {
            var p = GetPlane();
            var c = sphere.Center - Vector3.Dot(sphere.Center - (p.Normal * -p.D), p.Normal) * p.Normal;
            return ContainsCoplanar(c) &&
                   Vector3.Distance(sphere.Center, c) <= sphere.Radius;
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
        public bool ContainsCoplanar(Vector3 coplanarPoint)
        {
            Plane plane = GetPlane();
            
            Vector3[] _corners = GetCorners();
            if(_corners.Contains(coplanarPoint))
            {
                return true;
            }

            for (int i = 0; i < _corners.Length; i++)
            {                
                Vector3 p1 = coplanarPoint;
                Vector3 p2 = _corners[i];
                Vector3 p3 = i < _corners.Length - 1 ? _corners[i + 1] : _corners[0];
                Vector3 c = Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p2));
                if (c != Vector3.Normalize(plane.Normal) && c != Vector3.Zero)
                    return false;                
            }

            return true;
        }

        public bool Intersects(BoundingQuadOA quad)
        {
            foreach (var corner in quad.GetCorners())
            {
                if (ContainsCoplanar(corner))
                {
                    return true;
                }
            }

            GetEdges(out Vector3[] thisEdgePointsA, out Vector3[] thisEdgePointsB);
            var thisRays = thisEdgePointsA.Select((p, i) => new Ray(p, Vector3.Normalize(thisEdgePointsB[i] - p))).ToArray();
            for (int i = 0; i < thisRays.Length; i++)
            {
                if (quad.Intersects(thisRays[i]))
                {
                    var intersectionTypes = new[]
                    {
                        quad.GetPlane().Intersects(ref thisEdgePointsA[i]),
                        quad.GetPlane().Intersects(ref thisEdgePointsB[i]),
                    };

                    if (intersectionTypes.Contains(PlaneIntersectionType.Back) &&
                        intersectionTypes.Contains(PlaneIntersectionType.Front))
                    {
                        return true;
                    }
                }
            }

            quad.GetEdges(out Vector3[] quadEdgePointsA, out Vector3[] quadEdgePointsB);
            var quadRays = quadEdgePointsA.Select((p, i) => new Ray(p, Vector3.Normalize(quadEdgePointsB[i] - p))).ToArray();
            for (int i = 0; i < quadRays.Length; i++)
            {
                if (Intersects(quadRays[i]))
                {
                    var intersectionTypes = new[]
                    {
                        GetPlane().Intersects(ref thisEdgePointsA[i]),
                        GetPlane().Intersects(ref thisEdgePointsB[i]),
                    };

                    if (intersectionTypes.Contains(PlaneIntersectionType.Back) &&
                        intersectionTypes.Contains(PlaneIntersectionType.Front))
                    {
                        return true;
                    }
                }
            }

            return false;
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

        /// <summary>
        /// returns edges in clockwise order, starting with left edge, ending with bottom edge.
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        public void GetEdges(out Vector3[] pointA, out Vector3[] pointB)
        {
            pointA = new Vector3[4];
            pointB = new Vector3[4];
            
            //left edge
            pointA[0] = BottomLeft;
            pointB[0] = TopLeft;
            //top edge
            pointA[1] = TopLeft;
            pointB[1] = TopRight;
            //right edge
            pointA[2] = TopRight;
            pointB[2] = BottomRight;
            //bottom edge
            pointA[3] = BottomRight;
            pointB[3] = BottomLeft;           
        }

        public override string ToString()
        {
            return string.Format("TopLeft: {0}, TopRight: {1}, BottomRight: {2}, BottomLeft: {3}",
                                  TopLeft, TopRight, BottomRight, BottomLeft);
        }
    }
    
    /// <summary>
    /// </summary>
    public enum QuadIntersectionType
    {
        Back,
        Front,
        CoplanarExterior,
        Intersects
    }
}
